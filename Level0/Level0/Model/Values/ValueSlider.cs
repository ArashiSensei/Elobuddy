using LevelZero.Model.Enuns;

namespace LevelZero.Model.Values
{
    class ValueSlider : ValueAbstract
    {
        public int InitialValue { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public ValueSlider(int maxValue, int minValue, int initialValue, string indentifier, string displayName, bool separatorBefore = false)
        {
            DisplayName = displayName;
            Identifier = indentifier;
            InitialValue = initialValue;
            MaxValue = maxValue;
            MinValue = minValue;
            EnumMenuStyle = EnumMenuStyle.Slider;
            SeparatorBefore = separatorBefore;
        }
    }
}
