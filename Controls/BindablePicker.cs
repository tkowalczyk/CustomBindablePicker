namespace CustomBindablePicker.Controls
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Xamarin.Forms;

	/// <summary>
	/// Most of the credit for this control goes these two blog posts:  
	/// https://forums.xamarin.com/discussion/63565/xamarin-forms-picker-data-binding-using-mvvm
	/// https://forums.xamarin.com/discussion/30801/xamarin-forms-bindable-picker
	/// 
	/// I added in SelectedValue & SelectedValuePath and internal control logic
	/// </summary>
	public class BindablePicker : Picker
	{
		Boolean _disableNestedCalls;

		public String DisplayMemberPath { get; set; }

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public Object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set
			{
				if (SelectedItem != value)
				{
					SetValue(SelectedItemProperty, value);
					InternalSelectedItemChanged();
				}
			}
		}

		public Object SelectedValue
		{
			get { return GetValue(SelectedValueProperty); }
			set
			{
				SetValue(SelectedValueProperty, value);
				InternalSelectedValueChanged();
			}
		}

		public String SelectedValuePath { get; set; }

		public BindablePicker()
		{
			this.SelectedIndexChanged += OnSelectedIndexChanged;
		}

		public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindablePicker),
				null, propertyChanged: OnItemsSourceChanged);

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create("SelectedItem", typeof(Object), typeof(BindablePicker),
				null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

		public static readonly BindableProperty SelectedValueProperty =
			BindableProperty.Create("SelectedValue", typeof(Object), typeof(BindablePicker),
				null, BindingMode.TwoWay, propertyChanged: OnSelectedValueChanged);

		void InternalSelectedItemChanged()
		{
			if (_disableNestedCalls)
			{
				return;
			}

			var selectedIndex = -1;
			Object selectedValue = null;
			if (ItemsSource != null)
			{
				var index = 0;
				var hasSelectedValuePath = !String.IsNullOrWhiteSpace(SelectedValuePath);
				foreach (var item in ItemsSource)
				{
					if (item != null && item.Equals(SelectedItem))
					{
						selectedIndex = index;
						if (hasSelectedValuePath)
						{
							var type = item.GetType();
							var prop = type.GetRuntimeProperty(SelectedValuePath);
							selectedValue = prop.GetValue(item);
						}
						break;
					}
					index++;
				}
			}
			_disableNestedCalls = true;
			SelectedValue = selectedValue;
			SelectedIndex = selectedIndex;
			_disableNestedCalls = false;
		}

		void InternalSelectedValueChanged()
		{
			if (_disableNestedCalls)
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(SelectedValuePath))
			{
				return;
			}
			var selectedIndex = -1;
			Object selectedItem = null;
			var hasSelectedValuePath = !String.IsNullOrWhiteSpace(SelectedValuePath);
			if (ItemsSource != null && hasSelectedValuePath)
			{
				var index = 0;
				foreach (var item in ItemsSource)
				{
					if (item != null)
					{
						var type = item.GetType();
						var prop = type.GetRuntimeProperty(SelectedValuePath);
						if (prop.GetValue(item) == SelectedValue)
						{
							selectedIndex = index;
							selectedItem = item;
							break;
						}
					}

					index++;
				}
			}
			_disableNestedCalls = true;
			SelectedItem = selectedItem;
			SelectedIndex = selectedIndex;
			_disableNestedCalls = false;
		}

		static void OnItemsSourceChanged(BindableObject bindable, Object oldValue, Object newValue)
		{
			if (Equals(newValue, null) && Equals(oldValue, null))
			{
				return;
			}

			var picker = (BindablePicker)bindable;
			picker.Items.Clear();

			if (!Equals(newValue, null))
			{
				var hasDisplayMemberPath = !String.IsNullOrWhiteSpace(picker.DisplayMemberPath);

				foreach (var item in (IEnumerable)newValue)
				{
					if (hasDisplayMemberPath)
					{
						var type = item.GetType();
						var prop = type.GetRuntimeProperty(picker.DisplayMemberPath);
						picker.Items.Add(prop.GetValue(item).ToString());
					}
					else {
						picker.Items.Add(item.ToString());
					}
				}

				picker._disableNestedCalls = true;
				picker.SelectedIndex = -1;
				picker._disableNestedCalls = false;

				if (picker.SelectedItem != null)
				{
					picker.InternalSelectedItemChanged();
				}
				else if (hasDisplayMemberPath && picker.SelectedValue != null)
				{
					picker.InternalSelectedValueChanged();
				}
			}
			else {
				picker._disableNestedCalls = true;
				picker.SelectedIndex = -1;
				picker.SelectedItem = null;
				picker.SelectedValue = null;
				picker._disableNestedCalls = false;
			}
		}

		void OnSelectedIndexChanged(Object sender, EventArgs e)
		{
			if (_disableNestedCalls)
			{
				return;
			}

			if (SelectedIndex < 0 || ItemsSource == null || !ItemsSource.GetEnumerator().MoveNext())
			{
				_disableNestedCalls = true;
				if (SelectedIndex != -1)
				{
					SelectedIndex = -1;
				}
				SelectedItem = null;
				SelectedValue = null;
				_disableNestedCalls = false;
				return;
			}

			_disableNestedCalls = true;

			var index = 0;
			var hasSelectedValuePath = !String.IsNullOrWhiteSpace(SelectedValuePath);
			foreach (var item in ItemsSource)
			{
				if (index == SelectedIndex)
				{
					SelectedItem = item;
					if (hasSelectedValuePath)
					{
						var type = item.GetType();
						var prop = type.GetRuntimeProperty(SelectedValuePath);
						SelectedValue = prop.GetValue(item);
					}

					break;
				}
				index++;
			}

			_disableNestedCalls = false;
		}

		static void OnSelectedItemChanged(BindableObject bindable, Object oldValue, Object newValue)
		{
			var boundPicker = (BindablePicker)bindable;
			boundPicker.ItemSelected?.Invoke(boundPicker, new SelectedItemChangedEventArgs(newValue));
			boundPicker.InternalSelectedItemChanged();
		}

		static void OnSelectedValueChanged(BindableObject bindable, Object oldValue, Object newValue)
		{
			var boundPicker = (BindablePicker)bindable;
			boundPicker.InternalSelectedValueChanged();
		}

	}
}