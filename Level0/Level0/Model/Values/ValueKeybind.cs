using System;
using EloBuddy.SDK.Menu.Values;
using LevelZero.Model.Enuns;

namespace LevelZero.Model.Values
{
    class ValueKeybind : ValueAbstract
    {
        public bool InitialValue { get; set; }
        public KeyBind.BindTypes BindType { get; set; }
        public Char Key { get; set; }

        public ValueKeybind(bool initialValue, string identifier, string displayName, KeyBind.BindTypes bindType = KeyBind.BindTypes.HoldActive, Char key = '\0', bool separatorBefore = false)
        {
            Identifier = identifier;
            DisplayName = displayName;
            InitialValue = initialValue;
            BindType = bindType;
            EnumMenuStyle = EnumMenuStyle.KeyBind;
            key = Key;
            SeparatorBefore = separatorBefore;
        }
    }
}
