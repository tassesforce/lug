using System.Collections;
using System.Collections.Generic;

namespace lug.Enum
{
    /// <summary> Classe mere des enum avec pour valeur une string plutot qu'un int</summary>
    public abstract class StringEnum
    {
        /// <summary> display text de l'enum</summary>
        public string Text {get; private set;}
        /// <summary> Valeur de l'enum</summary>
        public string Value {get; private set;}

        protected StringEnum(string value, string text){
            this.Text = text;
            this.Value = value;
        }

        public StringEnum()
        {
        }

        /// <summary>Retourne la liste de tous les elements de l'enum</summary>
        public abstract List<StringEnum> ToList();

        /// <summary> Return l'element de l'enum qui correspond a la valeur en parametre</summary>
        public StringEnum ValueOf(string value)
        {
            foreach(StringEnum c in ToList())
            {
                if (c.Value.Equals(value))
                {
                    return c;
                }
            }
            return null;
        }

    }
}