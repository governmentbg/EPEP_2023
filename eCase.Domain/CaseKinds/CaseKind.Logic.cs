using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class CaseKind
    {
        public string Abbreviation
        {
            get
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>()
                {
                    {"2001", "НОХД"},
                    {"2002", "НЧХД"},
                    {"2003", "НАХД"},
                    {"2004", "ЧНД"},
                    {"2005", "ВНОХД"},
                    {"2006", "ВНЧХД"},
                    {"2007", "КНАХД"},
                    {"2008", "ВЧНД"},
                    {"2009", "ВНАХД"},
                    {"2010", "ГД"},
                    {"2011", "ЧГД"},
                    {"2012", "ВГД"},
                    {"2013", "ВЧГД"},
                    {"2014", "ТД"},
                    {"2015", "ЧТД"},
                    {"2016", "ВТД"},
                    {"2017", "ВЧТД"},
                    {"2018", "АД"},
                    {"2019", "ЧАД"},
                    {"2020", "КНАД"},
                    {"2021", "ФД"},
                    {"2022", "БД"},
                    {"2023", "КГД"},
                    {"2024", "КЧГД"},
                    {"2025", "КТД"},
                    {"2026", "КЧТД"},
                    {"2027", "ДН"},
                    {"2028", "ТДН"},
                    {"2029", "ГДН"},
                    {"2030", "ГДН"},
                };

                if (dictionary.ContainsKey(this.Code))
                    return dictionary[this.Code];
                else
                    return string.Empty;
            }
        }
    }
}
