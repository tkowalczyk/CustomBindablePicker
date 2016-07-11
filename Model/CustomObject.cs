namespace CustomBindablePicker
{
	public class CustomObject
	{
		public int Key { get; set; }
		public string Value { get; set; }
		public string Abbreviation { get; set; }

		public override string ToString()
		{
			return Value;
		}
	}
}