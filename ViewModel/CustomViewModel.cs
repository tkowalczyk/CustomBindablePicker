namespace CustomBindablePicker
{
	using System.Collections.Generic;

	public class CustomViewModel
	{
		public IList<CustomObject> ObjectList { get; set; }

		public string SelectedValue { get; set; }

		public CustomObject StaticSelectedItem { get; set; }

		public CustomViewModel()
		{
			ObjectList = new List<CustomObject>()
			{
				new CustomObject()
				{
					Key = 1,
					Value = "one",
					Abbreviation = "o"
				},
				new CustomObject()
				{
					Key = 2,
					Value = "two",
					Abbreviation = "tw"
				},
				new CustomObject()
				{
					Key = 3,
					Value = "three",
					Abbreviation = "th"
				},
				new CustomObject()
				{
					Key = 4,
					Value = "four",
					Abbreviation = "f"
				}
			};

			this.StaticSelectedItem = ObjectList[1];
			this.SelectedValue = "tw";
		}
	}
}