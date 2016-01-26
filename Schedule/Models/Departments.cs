﻿using System;
using System.Collections.Generic;
using Schedule.Helpers;

namespace Schedule.Models
{
    public static class Departments
    {
        private static Dictionary<string, string> _Departments;

        static Departments()
        {
            _Departments = new Dictionary<string, string>
            {
                { "Экономика и организация производства", "Экономико-математический факультет" },
                { "Бухгалтерский учет, анализ и аудит", "Экономико-математический факультет" },
                { "Финансы и кредит", "Экономико-математический факультет" },
                { "Управление качеством", "Экономико-математический факультет" },
                { "Управление персоналом", "Экономико-математический факультет" },
                { "Экономическая теория", "Экономико-математический факультет" },
                { "Экономика и менеджмент", "Экономико-математический факультет" },
                { "Высшая математика", "Экономико-математический факультет" },
                { "Маркетинг", "Экономико-математический факультет" },
                { "Коммерция", "Экономико-математический факультет" },

                { "Физвоспитание", "Гуманитарный факультет" },
                { "Филология, издательское дело и редактирование", "Гуманитарный факультет" },
                { "Иностранные языки", "Гуманитарный факультет" },
                { "Политология, социология и связи с общественностью", "Гуманитарный факультет" },
                { "Философия", "Гуманитарный факультет" },
                { "История и культура", "Гуманитарный факультет" },
                { "Прикладная лингвистика", "Гуманитарный факультет" },

                { "Теоретическая и прикладная механика", "Строительный факультет" },
                { "Теплогазоснабжение и вентиляция", "Строительный факультет" },
                { "Архитектурно-строительное проектирование", "Строительный факультет" },
                { "Строительное производство и М", "Строительный факультет" },
                { "Строительные конструкции", "Строительный факультет" },

                { "Прикладная математика и информатика", "Факультет информационных систем и технологий" },
                { "Информационные системы", "Факультет информационных систем и технологий" },
                { "Измерительно-вычислительные комплексы", "Факультет информационных систем и технологий" },
                { "Вычислительная техника", "Факультет информационных систем и технологий" },

                { "Электроснабжение", "Энергетический факультет" },
                { "Теплоэнергетика", "Энергетический факультет" },
                { "Электропривод и автоматизация промышленных установок", "Энергетический факультет" },
                { "БЖД и промышленной экологии", "Энергетический факультет" },
                { "Химия", "Энергетический факультет" },

                { "Радиотехника, опто- и наноэлектроника", "Радиотехнический факультет" },
                { "Физика", "Радиотехнический факультет" },
                { "Телекоммуникации", "Радиотехнический факультет" },
                { "Проектирование и ТЭС", "Радиотехнический факультет" },
                { "Радиотехника", "Радиотехнический факультет" },

                { "Технология машиностроения", "Машиностроительный факультет" },
                { "Металлорежущие станки и инструменты", "Машиностроительный факультет" },
                { "Материаловедение и ОМД", "Машиностроительный факультет" },
                { "Автомобили", "Машиностроительный факультет" },
                { "Начертательная геометрия и машинная графика", "Машиностроительный факультет" },
                { "Основы проектирования машин", "Машиностроительный факультет" },

                { "КЭИ", "КЭИ" }
            };
        }

        public static string GetFaculty(string cathedra)
        {
            string result = "";
            try
            {
                result = _Departments[cathedra];
            }
            catch (Exception ex) {
                result = "(Отсутствует)";
                ErrorNotificationToMail.Warninig("Кафедра отсутствует в списке", cathedra);
                ErrorNotificationToMail.Error(ex);
            }

            return result;
        }
    }
}
