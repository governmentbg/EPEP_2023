using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VSS.Portal.Models
{
    public static class CourtMetadata
    {
        public readonly static Court Court100 = new Court("100", "Апелативен Съд София", "Апелативен Съд", "София", "1000", "бул. Витоша 2", "02 921988", "registry@appellatecs.org", "https://sofia-as.justice.bg", 42.6954187, 23.3205029);
        
        
        //public readonly static Court Court101 = new Court("101", "Апелативен специализиран наказателен съд", "Апелативен специализиран наказателен съд", "София", "1505", "ул. Черковна 90", "02 4217204", "asns@asns-sofia.bg", "https://aspns.justice.bg", 42.6915394, 23.3510483);
        //public readonly static Court Court105 = new Court("105", "Специализиран наказателен съд", "Специализиран наказателен съд", "София", "1505", "ул. Черковна 90", "02 8140573", "ctrifonova@spcc.bg", "https://spns.justice.bg", 42.6915394, 23.3510483);
        //Закриване на специализирани съдилища, 28.07.2022
        public readonly static Court Court101 = new Court("101", "Апелативен съд - София - АСНС закрит", "Апелативен съд - София - АСНС закрит", "София", "1505", "ул. Черковна 90", "02 4217204", "asns@asns-sofia.bg", "https://aspns.justice.bg", 42.6915394, 23.3510483);
        public readonly static Court Court105 = new Court("105", "Софийски градски съд - СНС закрит", "Софийски градски съд - СНС закрит", "София", "1505", "ул. Черковна 90", "02 8140573", "ctrifonova@spcc.bg", "https://spns.justice.bg", 42.6915394, 23.3510483);



        public readonly static Court Court110 = new Court("110", "Градски Съд София", "Софийски Градски Съд", "София", "1000", "бул. Витоша 2", "02 9219332", "adm.sgs@scc.bg", "https://sgs.justice.bg", 42.6954187, 23.3205029);

        //public readonly static Court Court111 = new Court("111", "Районен Съд София", "Районен Съд", "София", "1164", "бул. Драган Цанков 6", "02 8955", "sofia_rs@netissat.bg", "https://sofia-rs.justice.bg", 42.6840812, 23.3320981);
        public readonly static Court Court1111 = new Court("1111", "Районен Съд София - НО", "Районен Съд", "Наказателно отделение, София", "1463", "бул. „Генерал Скобелев“ 23", "02 8955231", "documents.no@srs.justice.bg", "https://www.srs.justice.bg", 42.6879766, 23.3144902);
        public readonly static Court Court1112 = new Court("1112", "Районен Съд София - ГО" , "Районен Съд", "Гражданско отделение, София", "1612", "бул. „Цар Борис III“ 54", "02 8955600", "documents.go@srs.justice.bg", "https://www.srs.justice.bg", 42.6844624, 23.2943629);


        public readonly static Court Court120 = new Court("120", "Окръжен Съд Благоевград", "Окръжен Съд", "Благоевград", "2700", "пл. Васил Левски 1", "073 889840", "court@pirin.com", "https://blagoevgrad-os.justice.bg", 42.0185551, 23.0969757);
        public readonly static Court Court121 = new Court("121", "Районен съд Благоевград", "Районен Съд", "Благоевград", "2700", "пл. Васил Левски 1", "073 8896424", "rc_blagoevgrad@mail.bg", "https://blagoevgrad-rs.justice.bg", 42.0185551, 23.0969757);
        public readonly static Court Court122 = new Court("122", "Районен Съд Гоце Делчев", "Районен Съд", "Гоце Делчев", "2900", "ул. Отец Паисий 25", "0751 60181", "court_gd@abv.bg", "https://gdelchev-rs.justice.bg", 41.57046756, 23.71548608);
        public readonly static Court Court123 = new Court("123", "Районен Съд Петрич", "Районен Съд", "Петрич", "2850", "ул. Лазар Маджаров N:3", "074522053", "rspetrich@gmail.com", "https://petrich-rs.justice.bg", 41.3923636, 23.2090436);
        public readonly static Court Court124 = new Court("124", "Районен Съд Разлог", "Районен Съд", "Разлог", "2760", "ул. И. Макариополски 23", "0747 80181", "rs_razlog@abv.bg", "https://razlog-rs.justice.bg", 41.8882853, 23.4638382);
        public readonly static Court Court125 = new Court("125", "Районен Съд Сандански", "Районен Съд", "Сандански", "2800", "Бул. Македония 57", "0746 34561", "rs_sandanski@abv.bg", "https://sandanski-rs.justice.bg", 41.565481, 23.280274);
        public readonly static Court Court130 = new Court("130", "Окръжен Съд Видин", "Окръжен Съд", "Видин", "3700", "пл. Бдинци 1", "094 600980", "os.vidin@abv.bg", "https://vidin-os.justice.bg", 43.9864407, 22.8772471);
        public readonly static Court Court131 = new Court("131", "Районен Съд Белоградчик", "Районен Съд", "Белоградчик", "3900", "ул. Княз Борис І 45", "093 654168", "rsbelog@abv.bg", "https://belogradchik-rs.justice.bg", 43.6259886, 22.6835531);
        public readonly static Court Court132 = new Court("132", "Районен Съд Видин", "Районен Съд", "Видин", "3700", "пл. Бдинци 1", "094 600782", "rs_vidin@yahoo.com", "https://vidin-rs.justice.bg", 43.9864407, 22.8772471);
        public readonly static Court Court133 = new Court("133", "Районен Съд Кула", "Районен Съд", "Кула", "3800", "ул. Иван Кръстев 1", "0938 3149", "rskula@abv.bg", "https://kula-rs.justice.bg", 43.8897418, 22.522318);
        public readonly static Court Court140 = new Court("140", "Окръжен Съд Враца", "Окръжен Съд", "Враца", "3000", "бул. Христо Ботев 29", "092 682100", "os.vraca@abv.bg", "https://vratsa-os.justice.bg", 43.2041424, 23.5486075);
        public readonly static Court Court141 = new Court("141", "Районен Съд Бяла Слатина", "Районен Съд", "Бяла Слатина", "3200", "ул. Димитър Благоев 85", "0892255060", "sadbs@mail.orbitel.bg", "https://bslatina-rs.justice.bg", 43.4724714, 23.9325363);
        public readonly static Court Court142 = new Court("142", "Районен Съд Враца", "Районен Съд", "Враца", "3000", "бул. Христо Ботев 29", "092 682144", "rs.vratza@abv.bg", "https://vratsa-rs.justice.bg", 43.2041424, 23.5486075);
        public readonly static Court Court143 = new Court("143", "Районен Съд Кнежа", "Районен Съд", "Кнежа", "5835", "ул. Марин Боев 71", "09132 7236", "rs_kneja@abv.bg", "https://knezha-rs.justice.bg", 43.496346, 24.079434);
        public readonly static Court Court144 = new Court("144", "Районен Съд Козлодуй", "Районен Съд", "Козлодуй", "3320", "ул. Кирил и Методий 5", "0973 80676", "rskozlodui@abv.bg", "https://kozloduy-rs.justice.bg", 43.7678448, 23.7262949);
        public readonly static Court Court145 = new Court("145", "Районен Съд Мездра", "Районен Съд", "Мездра", "3100", "ул. Св.Св. Кирил и Методий 21", "0910 92548", "rsud_mz@abv.bg", "https://mezdra-rs.justice.bg", 43.1450626, 23.7166548);
        public readonly static Court Court146 = new Court("146", "Районен Съд Оряхово", "Районен Съд", "Оряхово", "3300", "ул. Арх. Цолов 47", "09171 3065", "rsoriahovo@abv.bg", "https://oryahovo-rs.justice.bg", 43.732652, 23.959437);
        public readonly static Court Court150 = new Court("150", "Окръжен Съд Кюстендил", "Окръжен Съд", "Кюстендил", "2500", "ул. Гороцветна 31", "078 550455", "kos@kos-bg.eu", "https://kyustendil-os.justice.bg", 42.2781306, 22.6878483);
        public readonly static Court Court151 = new Court("151", "Районен Съд Дупница", "Районен Съд", "Дупница", "2600", "ул. Николаевска 15", "0701 52076", "drs@mbox.contact.bg", "https://dupnitsa-rs.justice.bg", 42.265949, 23.1229308);
        public readonly static Court Court152 = new Court("152", "Районен Съд Кюстендил", "Районен Съд", "Кюстендил", "2500", "ул. Гороцветна 31", "078 550789", "krs@krs-bg.eu", "https://kyustendil-rs.justice.bg", 42.2781306, 22.6878483);
        public readonly static Court Court160 = new Court("160", "Окръжен Съд Монтана", "Окръжен Съд", "Монтана", "3400", "ул. Васил Левски 24", "096 395101", "mail@court-montana.com", "https://montana-os.justice.bg", 43.4054713, 23.2256892);
        public readonly static Court Court161 = new Court("161", "Районен Съд Берковица", "Районен Съд", "Берковица", "3500", "пл. Йордан Радичков 2", "0953 88817", "brs@mail.bg", "https://berkovitsa-rs.justice.bg", 43.2377348, 23.1235786);
        public readonly static Court Court162 = new Court("162", "Районен Съд Лом", "Районен Съд", "Лом", "3600", "пл. Свобода 8", "0971 68101", "court@rclom.org", "https://lom-rs.justice.bg", 43.82562, 23.237503);
        public readonly static Court Court163 = new Court("163", "Районен Съд Монтана", "Районен Съд", "Монтана", "3400", "ул. Васил Левски 22", "096 395160", "montana_sad@mail.bg", "https://montana-rs.justice.bg", 43.4064895, 23.226449);
        public readonly static Court Court170 = new Court("170", "Окръжен Съд Перник", "Окръжен Съд", "Перник", "2300", "ул. Търговска 37", "076647012", "pernik-os@justice.bg", "https://pernik-os.justice.bg", 42.6021866, 23.0299629);
        public readonly static Court Court171 = new Court("171", "Районен Съд Брезник", "Районен Съд", "Брезник", "2360", "ул. Пролетарска 4", "07779 8748", "rs_breznik@mail.bg", "https://breznik-rs.justice.bg", 42.7412596, 22.9080799);
        public readonly static Court Court172 = new Court("172", "Районен Съд Перник", "Районен Съд", "Перник", "2300", "ул. Търговска 37", "076 647502", "rspernik@abv.bg", "https://pernik-rs.justice.bg", 42.6021866, 23.0299629);
        public readonly static Court Court173 = new Court("173", "Районен Съд Радомир", "Районен Съд", "Радомир", "2400", "ул. Свети Свети Кирил и Методи 22", "0777 84031", "rs_radomir@abv.bg", "https://radomir-rs.justice.bg", 42.5481276, 22.9656279);
        public readonly static Court Court174 = new Court("174", "Районен Съд Трън", "Районен Съд", "Трън", "2460", "ул. Г. Димитров 4", "0777 98652", "rstran@abv.bg", "https://tran-rs.justice.bg", 42.8343426, 22.6503696);
        public readonly static Court Court180 = new Court("180", "Окръжен Съд София", "Окръжен Съд", "София", "1000", "бул. Витоша 2", "02 9219203", "sd_court@sofiadc.justice.bg", "https://sofia-os.justice.bg", 42.6954187, 23.3205029);
        public readonly static Court Court181 = new Court("181", "Районен Съд Ботевград", "Районен Съд", "Ботевград", "2140", "ул. Свобода 3Б", "0723 69349", "rajsud@abv.bg", "https://botevgrad-rs.justice.bg", 42.9087742, 23.7896574);
        public readonly static Court Court182 = new Court("182", "Районен Съд Елин Пелин", "Районен Съд", "Елин Пелин", "2100", "ул. Владимир Заимов 1", "0725 60172", "rs_elinpelin@abv.bg", "https://elpelin-rs.justice.bg", 42.669374, 23.599497);
        public readonly static Court Court183 = new Court("183", "Районен Съд Етрополе", "Районен Съд", "Етрополе", "2180", "бул. Руски 105", "0720 63758", "etropole-rs@justice.bg", "https://etropole-rs.justice.bg", 42.8340305, 23.9978083);
        public readonly static Court Court184 = new Court("184", "Районен Съд Ихтиман", "Районен Съд", "Ихтиман", "2050", "ул. Иван Вазов 1", "0724 87032", "predsedatel@rsihtiman.org", "https://ihtiman-rs.justice.bg", 42.440504, 23.814641);
        public readonly static Court Court185 = new Court("185", "Районен Съд Костинброд", "Районен Съд", "Костинброд", "2231", "ул.Детелина 2А", "0721 66138", "rs_kostinbrod@abv.bg", "https://kostinbrod-rs.justice.bg", 42.809994, 23.200482);
        public readonly static Court Court186 = new Court("186", "Районен Съд Пирдоп", "Районен Съд", "Пирдоп", "2070", "бул. Цар Освободител 47", "07181 5970", "rspirdop@abv.bg", "https://pirdop-rs.justice.bg", 42.6999952, 24.1775973);
        public readonly static Court Court187 = new Court("187", "Районен Съд Самоков", "Районен Съд", "Самоков", "2000", "ул. Бачо Киро 1", "0722 66125", "samokov-rs@justice.bg", "https://samokov-rs.justice.bg", 42.3378602, 23.5617674);
        public readonly static Court Court188 = new Court("188", "Районен Съд Своге", "Районен Съд", "Своге", "2260", "ул. Александър Вутимски 9", "0726 22008", "rs_svoge@abv.bg", "https://svoge-rs.justice.bg", 42.9595739, 23.3498974);
        public readonly static Court Court189 = new Court("189", "Районен Съд Сливница", "Районен Съд", "Сливница", "2200", "пл. Съединение 1", "0727 42247", "rs_slivnitza@abv.bg", "https://slivnitsa-rs.justice.bg", 42.8519742, 23.0378716);
        public readonly static Court Court200 = new Court("200", "Апелативен Съд Бургас", "Апелативен Съд", "Бургас", "8000", "ул. Александровска 101", "056 859919", "ap.sad.burgas@abv.bg", "https://burgas-as.justice.bg", 42.4983556, 27.4711177);
        public readonly static Court Court210 = new Court("210", "Окръжен Съд Бургас", "Окръжен Съд", "Бургас", "8000", "ул. Александровска 101", "056 879400", "judge@os-burgas.org", "https://burgas-os.justice.bg/", 42.4983556, 27.4711177);
        public readonly static Court Court211 = new Court("211", "Районен Съд Айтос", "Районен Съд", "Айтос", "8500", "ул. Цар Освободител 3А", "055 22016", "rs_aitos@abv.bg", "https://aytos-rs.justice.bg", 42.703056, 27.254387);
        public readonly static Court Court212 = new Court("212", "Районен Съд Бургас", "Районен Съд", "Бургас", "8000", "ул. Александровска 101", "056 878843", "rsyd_burgas@abv.bg", "https://burgas-rs.justice.bg", 42.498273, 27.470817);
        public readonly static Court Court213 = new Court("213", "Районен Съд Карнобат", "Районен Съд", "Карнобат", "8400", "ул. Г. Димитров 2", "0559 25915", "rs_karnobat@mail.bg", "https://karnobat-rs.justice.bg", 42.647336, 26.984159);
        public readonly static Court Court214 = new Court("214", "Районен Съд Малко Търново", "Районен Съд", "Малко Търново", "8162", "ул. Райна княгиня 3", "05952 3652", "mtrs@abv.bg", "https://mtarnovo-rs.justice.bg", 42.4888785, 26.507687);
        public readonly static Court Court215 = new Court("215", "Районен Съд Несебър", "Районен Съд", "Несебър", "8230", "ул. Иван Вазов 23", "0554 43254", "rsnesebar@gmail.com", "https://nesebar-rs.justice.bg", 42.6593223, 27.7183723);
        public readonly static Court Court216 = new Court("216", "Районен Съд Поморие", "Районен Съд", "Поморие", "8200", "ул. Калоян 2", "0596 22072", "rspomorie@mail.bg", "https://pomorie-rs.justice.bg", 42.5578, 27.6397);
        public readonly static Court Court217 = new Court("217", "Районен Съд Средец", "Районен Съд", "Средец", "8300", "пл. Г. Димитров 3", "05551 7373", "sredecsad@abv.bg", "https://sredets-rs.justice.bg", 42.346943, 27.181465);
        public readonly static Court Court218 = new Court("218", "Районен Съд Царево", "Районен Съд", "Царево", "8260", "ул. Крайморска 26", "0590 53520", "rs_carevo@abv.bg", "https://tsarevo-rs.justice.bg", 42.170204, 27.849629);
        public readonly static Court Court220 = new Court("220", "Окръжен Съд Сливен", "Окръжен Съд", "Сливен", "8800", "бул. Г. Данчев 1", "044 616403", "os_sliven@iradeum.com", "https://sliven-os.justice.bg", 42.678453, 26.3264363);
        public readonly static Court Court221 = new Court("221", "Районен Съд Котел", "Районен Съд", "Котел", "8970", "ул. Раковска 58", "0453 42476", "sad.kotel@gmail.com", "https://kotel-rs.justice.bg/", 42.8845022, 26.4485315);
        public readonly static Court Court222 = new Court("222", "Районен Съд Нова Загора", "Районен Съд", "Нова Загора", "8900", "ул. Проф. Минко Балкански 60", "0457 62994", "nzagora-rs@justice.bg", "https://nzagora-rs.justice.bg", 42.491889, 26.015684);
        public readonly static Court Court223 = new Court("223", "Районен Съд Сливен", "Районен Съд", "Сливен", "8800", "пл. Хаджи Димитър 2", "044 624091", "rs_sliven@blizoomail.bg", "https://sliven-rs.justice.bg", 42.6809716, 26.3135309);
        public readonly static Court Court230 = new Court("230", "Окръжен Съд Ямбол", "Окръжен Съд", "Ямбол", "8600", "ул. Жорж Папазов 1 1", "046 688822", "info@os-yambol.org", "https://yambol-os.justice.bg", 42.4850413, 26.5053682);
        public readonly static Court Court231 = new Court("231", "Районен Съд Елхово", "Районен Съд", "Елхово", "8700", "ул. Пирот 2", "0478 81362", "elhovo-rs@justice.bg", "https://elhovo-rs.justice.bg", 42.17503, 26.57356);
        public readonly static Court Court232 = new Court("232", "Районен Съд Тополовград", "Районен Съд", "Тополовград", "6560", "ул. Иван Вазов 2", "0470 2959", "topol_bg@abv.bg", "https://topolovgrad-rs.justice.bg", 42.085274, 26.33856);
        public readonly static Court Court233 = new Court("233", "Районен Съд Ямбол", "Районен Съд", "Ямбол", "8600", "ул. Жорж Папазов 1", "046 661829", "yrs@yambollan.com", "https://yambol-rs.justice.bg", 42.4850413, 26.5053682);
        public readonly static Court Court300 = new Court("300", "Апелативен Съд Варна", "Апелативен Съд", "Варна", "8000", "пл. Независимост 2", "052 686506", "vnaps@mbox.contact.bg", "https://varna-as.justice.bg", 43.2043147, 27.9121643);
        public readonly static Court Court310 = new Court("310", "Окръжен Съд Варна", "Окръжен Съд", "Варна", "9000", "пл. Независимост 2", "052 622062", "president@vos.bg", "https://varna-os.justice.bg", 43.2043147, 27.9121643);
        public readonly static Court Court311 = new Court("311", "Районен Съд Варна", "Районен Съд", "Варна", "9000", "бул. Владислав Варненчик 57", "052 602028", "vrc@mbox.contact.bg", "https://varna-rs.justice.bg", 43.211377, 27.9063091);
        public readonly static Court Court312 = new Court("312", "Районен Съд Девня", "Районен Съд", "Девня", "9160", "ул. Строител 12", "0519 92888", "rs_devnya@courtdevnya.org", "https://devnya-rs.justice.bg", 43.217120, 27.603484);
        public readonly static Court Court313 = new Court("313", "Районен Съд Провадия", "Районен Съд", "Провадия", "9200", "ул. Александър Стамболийски 23", "051 847733", "rsprov@court-provadia.com", "https://provadiya-rs.justice.bg", 43.181847, 27.44465);
        public readonly static Court Court320 = new Court("320", "Окръжен Съд Добрич", "Окръжен Съд", "Добрич", "9300", "ул. Д-р Константин Стоилов 7", "058 652030", "info@os-dobrich.com", "https://dobrich-os.justice.bg", 43.5719477, 27.8293558);
        public readonly static Court Court321 = new Court("321", "Районен Съд Балчик", "Районен Съд", "Балчик", "9600", "ул. Стара планина 2", "579 75044", "balchik_rs@abv.bg", "https://balchik-rs.justice.bg", 43.4225685, 28.1686931);
        public readonly static Court Court322 = new Court("322", "Районен Съд Генерал Тошево", "Районен Съд", "Генерал Тошево", "9500", "ул. Опълченска1", "05731 2078", "gtrs@mail.bg", "https://gtoshevo-rs.justice.bg", 43.7008, 28.036);
        public readonly static Court Court323 = new Court("323", "Районен Съд Добрич", "Районен Съд", "Добрич", "9300", "ул. Д-р Константин Стоилов 7", "058 652020", "rs_dobrich@abv.bg", "https://dobrich-rs.justice.bg", 43.5719477, 27.8293558);
        public readonly static Court Court324 = new Court("324", "Районен Съд Каварна", "Районен Съд", "Каварна", "9650", "ул. Дончо Стойков 8", "0570 81015", "rs_kavarna@abv.bg", "https://kavarna-rs.justice.bg", 43.4341158, 28.3401855);
        public readonly static Court Court325 = new Court("325", "Районен Съд Тервел", "Районен Съд", "Тервел", "9450", "ул. Христо Ботев 3", "05751 4043", "rstervel@mail.bg", "https://tervel-rs.justice.bg", 43.748681, 27.408500);
        public readonly static Court Court330 = new Court("330", "Окръжен Съд Разград", "Окръжен Съд", "Разград", "7200", "пл. Независимост 1", "084 660537", "os_razgrad@mail.bg", "https://razgrad-os.justice.bg/", 43.5267171, 26.5234712);
        public readonly static Court Court331 = new Court("331", "Районен Съд Исперих", "Районен Съд", "Исперих", "7400", "ул. Дунав 2", "08431 2076", "rs_isperih@raionensydisperih.org", "https://isperih-rs.justice.bg", 43.72172, 26.833782);
        public readonly static Court Court332 = new Court("332", "Районен Съд Кубрат", "Районен Съд", "Кубрат", "7300", "ул. Цар Иван Асен ІІ 4", "0838 72563", "rs_kubrat@abv.bg", "https://kubrat-rs.justice.bg", 43.79759, 26.5058158);
        public readonly static Court Court333 = new Court("333", "Районен Съд Разград", "Районен Съд", "Разград", "7200", "пл. Независимост 1", "084 624 339", "rs_razgrad@mbox.contact.bg", "https://razgrad-rs.justice.bg", 43.5267171, 26.5234712);
        public readonly static Court Court340 = new Court("340", "Окръжен Съд Силистра", "Окръжен Съд", "Силистра", "7500", "ул. Симеон Велики 31", "086 816601", "sos_silistra@abv.bg", "https://silistra-os.justice.bg", 44.1186635, 27.2605987);
        public readonly static Court Court341 = new Court("341", "Районен Съд Дулово", "Районен Съд", "Дулово", "7650", "ул. В. Левски 12", "0864 24001", "rsdulovo@abv.bg", "https://dulovo-rs.justice.bg", 43.8199056, 27.1420925);
        
        public readonly static Court Court342 = new Court("342", "Районен Съд Силистра", "Районен Съд", "Силистра", "7500", "ул. Симеон Велики 31", "086 816501", "silistra-rs@vss.justice.bg", "https://silistra-rs.justice.bg", 44.1186635, 27.2605987);
        
        public readonly static Court Court343 = new Court("343", "Районен Съд Тутракан", "Районен Съд", "Тутракан", "7600", "ул. Трансмариска 8", "0857 61620", "RS343@mbox.contact.bg", "https://tutrakan-rs.justice.bg", 44.044787, 26.614034);
        public readonly static Court Court350 = new Court("350", "Окръжен Съд Търговище", "Окръжен Съд", "Търговище", "7700", "ул. П.Р.Славейков 59", "0601 62129", "info@justicetg.org", "https://targovishte-os.justice.bg", 43.2468777, 26.5736573);
        public readonly static Court Court351 = new Court("351", "Районен Съд Омуртаг", "Районен Съд", "Омуртаг", "7900", "ул. Цар Освободител 5", "0605 62423", "ors@abv.bg", "https://omurtag-rs.justice.bg", 43.1072865, 26.4188204);
        public readonly static Court Court352 = new Court("352", "Районен Съд Попово", "Районен Съд", "Попово", "7800", "ул.Ал. Стамболийски 1", "0608 47832", "info@rspopovo.com", "https://popovo-rs.justice.bg", 43.348255, 26.227152);
        public readonly static Court Court353 = new Court("353", "Районен Съд Търговище", "Районен Съд", "Търговище", "7700", "ул. Славейков 59", "0601 62410", "rs_tar@dir.bg", "https://targovishte-rs.justice.bg", 43.2468777, 26.5736573);
        public readonly static Court Court360 = new Court("360", "Окръжен Съд Шумен", "Окръжен Съд", "Шумен", "9700", "ул. Съединение 1", "054 850333", "shos@court-sh.org", "https://shumen-os.justice.bg/", 43.26583, 26.94791);
        public readonly static Court Court361 = new Court("361", "Районен Съд Велики Преслав", "Районен Съд", "Велики Преслав", "9850", "ул. Беломорска 80", "0538 48300", "vprs@abv.bg", "https://vpreslav-rs.justice.bg", 43.162029, 26.823506);
        public readonly static Court Court362 = new Court("362", "Районен Съд Нови Пазар", "Районен Съд", "Нови Пазар", "9900", "ул. Цар Освободител 31", "0537 27941", "rs_novipazar@abv.bg", "https://npazar-rs.justice.bg", 43.3436939, 27.1917548);
        public readonly static Court Court363 = new Court("363", "Районен Съд Шумен", "Районен Съд", "Шумен", "9700", "ул. Съединение 1", "054 850126", "rssh@court-sh.org", "https://shumen-rs.justice.bg", 43.26583, 26.94791);
        public readonly static Court Court400 = new Court("400", "Апелативен Съд Велико Търново", "Апелативен Съд", "Велико Търново", "5000", "ул. Васил Левски 16", "062 600730", "vtas@abv.bg", "https://vtarnovo-as.justice.bg", 43.0788842, 25.6271534);
        public readonly static Court Court410 = new Court("410", "Окръжен Съд Велико Търново", "Окръжен Съд", "Велико Търново", "5000", "ул. Васил Левски 16", "062 615889", "vtos.bg@gmail.com", "https://vtarnovo-os.justice.bg", 43.0788842, 25.6271534);
        public readonly static Court Court411 = new Court("411", "Районен Съд Велико Търново", "Районен Съд", "Велико Търново", "5000", "ул. Васил Левски 16", "062 615900", "vtrs@mail.bg", "https://vtarnovo-rs.justice.bg", 43.0788842, 25.6271534);
        public readonly static Court Court412 = new Court("412", "Районен Съд Горна Оряховица", "Районен Съд", "Горна Оряховица", "5100", "ул. Христо Ботев 2", "0618 61911", "rsad_go@abv.bg", "https://goryahovitsa-rs.justice.bg", 43.121998, 25.689562);
        public readonly static Court Court413 = new Court("413", "Районен Съд Елена", "Районен Съд", "Елена", "5070", "ул. Йеромонах Йосиф Брадати 2", "06151 6243", "rs_elena@abv.bg", "https://elena-rs.justice.bg", 42.9292248, 25.8748158);
        public readonly static Court Court414 = new Court("414", "Районен Съд Павликени", "Районен Съд", "Павликени", "5200", "ул. Атанас Хаджиславчев 8", "0610 51545", "rs_pavlikeni@abv.bg", "https://pavlikeni-rs.justice.bg", 43.2325, 25.29897);
        public readonly static Court Court415 = new Court("415", "Районен Съд Свищов", "Районен Съд", "Свищов", "5250", "ул. Димитър Анев 3", "0631 60293", "RS_Svishtov@abv.bg", "https://svishtov-rs.justice.bg", 43.6183951, 25.3450771);
        public readonly static Court Court420 = new Court("420", "Окръжен Съд Габрово", "Окръжен Съд", "Габрово", "5300", "пл. Възраждане 1", "066 811101", "justice@court-gbr.com", "https://gabrovo-os.justice.bg", 42.8709608, 25.3153538);
        public readonly static Court Court421 = new Court("421", "Районен Съд Габрово", "Районен Съд", "Габрово", "5300", "пл. Възраждане 1", "066 811207", "rs-gabrovo@coutr-gbr.com", "https://gabrovo-rs.justice.bg", 42.8709608, 25.3153538);
        public readonly static Court Court422 = new Court("422", "Районен Съд Дряново", "Районен Съд", "Дряново", "5370", "ул. Бачо Киро 21", "0676 75567", "raionen_sad_drianovo@abv.bg", "https://dryanovo-rs.justice.bg", 42.977926, 25.475724);
        public readonly static Court Court423 = new Court("423", "Районен Съд Севлиево", "Районен Съд", "Севлиево", "5400", "ул. Стефан Пешев 7", "0675 33880", "administrative@sevlievo.court-bg.org", "https://sevlievo-rs.justice.bg", 43.0252547, 25.1062142);
        public readonly static Court Court424 = new Court("424", "Районен Съд Трявна", "Районен Съд", "Трявна", "5350", "ул. Бачо Киро 1", "0677 62018", "rstryavna@mail.bg", "https://tryavna-rs.justice.bg", 42.866997, 25.491944);
        public readonly static Court Court430 = new Court("430", "Окръжен Съд Ловеч", "Окръжен Съд", "Ловеч", "5500", "ул. Търговска 41", "068 689898", "los@gbg.bg", "https://lovech-os.justice.bg", 43.1345201, 24.7167061);
        public readonly static Court Court431 = new Court("431", "Районен Съд Ловеч", "Районен Съд", "Ловеч", "5500", "ул. Търговска 41", "068 685525", "LRS@abv.bg", "https://lovech-rs.justice.bg", 43.1345201, 24.7167061);
        public readonly static Court Court432 = new Court("432", "Районен Съд Луковит", "Районен Съд", "Луковит", "5770", "ул. Раковски 6", "0697 2404", "rs_lukovit@abv.bg", "https://lukovit-rs.justice.bg", 43.2032051, 24.1648653);
        public readonly static Court Court433 = new Court("433", "Районен Съд Тетевен", "Районен Съд", "Тетевен", "5700", "ул. Христо Ботев 3А", "0678 54284", "rs_teteven@abv.bg", "https://teteven-rs.justice.bg", 42.916868, 24.264579);
        public readonly static Court Court434 = new Court("434", "Районен Съд Троян", "Районен Съд", "Троян", "5600", "пл. Възраждане 1", "0670 62952", "rs.troyan@mail.bg", "https://troyan-rs.justice.bg", 42.8841, 24.7103);
        public readonly static Court Court440 = new Court("440", "Окръжен Съд Плевен", "Окръжен Съд", "Плевен", "5800", "ул. Д. Константинов 25", "064 893913", "os_pleven@abv.bg", "https://pleven-os.justice.bg", 43.4086943, 24.6208884);
        public readonly static Court Court441 = new Court("441", "Районен Съд Левски", "Районен Съд", "Левски", "5900", "бул. България 58", "0650 82296", "levski_rs@abv.bg", "https://levski-rs.justice.bg/", 43.357482, 25.143098);
        public readonly static Court Court442 = new Court("442", "Районен Съд Никопол", "Районен Съд", "Никопол", "5940", "пл. Европа 14", "064 889 159", "nikopolrs@mbox.contact.bg", "https://nikopol-rs.justice.bg", 43.701476, 24.895006);
        public readonly static Court Court443 = new Court("443", "Районен Съд Плевен", "Районен Съд", "Плевен", "5800", "ул. Д. Константинов 25", "064 892975", "admin-sud-pleven@mbox.contact.bg", "https://pleven-rs.justice.bg", 43.4086943, 24.6208884);
        public readonly static Court Court444 = new Court("444", "Районен Съд Червен бряг", "Районен Съд", "Червен бряг", "5980", "ул. Екзарх Йосиф 6", "0659 93009", "chbryag-rs@justice.bg", "https://chbryag-rs.justice.bg", 43.2798151, 24.0795523);
        public readonly static Court Court450 = new Court("450", "Окръжен Съд Русе", "Окръжен Съд", "Русе", "7000", "ул. Александровска 57", "082 881273", "courtruse@gmail.com", "https://ruse-os.justice.bg", 43.8496899, 25.9534483);
        public readonly static Court Court451 = new Court("451", "Районен Съд Бяла", "Районен Съд", "Бяла", "7100", "Пл. Екзарх Йосиф I 6", "0817 73113", "rsbyala@rs-byala.org", "https://byala-rs.justice.bg", 43.4591, 25.737446);
        public readonly static Court Court452 = new Court("452", "Районен Съд Русе", "Районен Съд", "Русе", "7000", "ул. Александровска 57", "082 825430", "rusers@gmail.com", "https://ruse-rs.justice.bg", 43.8496899, 25.9534483);
        public readonly static Court Court500 = new Court("500", "Апелативен Съд Пловдив", "Апелативен Съд", "Пловдив", "4000", "бул. 6 септември 167", "032 656101", "pvapelsad@dir.bg", "https://plovdiv-as.justice.bg", 42.151972, 24.7475685);
        public readonly static Court Court510 = new Court("510", "Окръжен Съд Кърджали", "Окръжен Съд", "Кърджали", "6600", "бул. Беломорски 48", "0361 58815", "okrsad_kj@mail.bg", "https://kardzhali-os.justice.bg", 41.6395898, 25.3759015);
        public readonly static Court Court511 = new Court("511", "Районен Съд Ардино", "Районен Съд", "Ардино", "6750", "ул. Републиканска 6", "03651 4216", "ardino-rs@justice.bg", "https://ardino-rs.justice.bg", 41.583683, 25.13356);
        public readonly static Court Court512 = new Court("512", "Районен Съд Ивайловград", "Районен Съд", "Ивайловград", "6570", "ул. Г. Димитров 42", "03661 6017", "rsi@mail.bg", "https://ivaylovgrad-rs.justice.bg", 41.5270275, 26.1231075);
        public readonly static Court Court513 = new Court("513", "Районен Съд Крумовград", "Районен Съд", "Крумовград", "6900", "пл. България 17", "03641 7592", "sys.rskrumovgrad@gmail.com", "https://krumovgrad-rs.justice.bg", 41.470923, 25.653165);
        public readonly static Court Court514 = new Court("514", "Районен Съд Кърджали", "Районен Съд", "Кърджали", "6600", "бул. Беломорски 48", "0361 65190", "kardzhali-rs@justice.bg", "https://kardzhali-rs.justice.bg", 41.6395898, 25.3759015);
        public readonly static Court Court515 = new Court("515", "Районен Съд Момчилград", "Районен Съд", "Момчилград", "6800", "ул. Петър Мирчев 2", "03631 6003", "rs@link.bg", "https://momchilgrad-rs.justice.bg", 41.520403, 25.413035);
        public readonly static Court Court520 = new Court("520", "Окръжен Съд Пазарджик", "Окръжен Съд", "Пазарджик", "4400", "ул. Хан Крум 3", "034 409500", "pazardzhik-os@justice.bg", "https://pazardzhik-os.justice.bg", 42.1945089, 24.3329829);
        public readonly static Court Court521 = new Court("521", "Районен Съд Велинград", "Районен Съд", "Велинград", "4600", "ул. Хан Аспарух 3", "0359 55551", "rs_vel@abv.bg", "https://velingrad-rs.justice.bg", 42.0280504, 23.9959362);
        public readonly static Court Court522 = new Court("522", "Районен Съд Пазарджик", "Районен Съд", "Пазарджик", "4400", "ул. Хан Крум 3", "034 409600", "rcourt-pz@rcourt-pz.info", "https://pazardzhik-rs.justice.bg", 42.1945089, 24.3329829);
        public readonly static Court Court523 = new Court("523", "Районен Съд Панагюрище", "Районен Съд", "Панагюрище", "4500", "ул. Петко Мачев 2", "035788839", "rspan@mail.bg", "https://panagyurishte-rs.justice.bg", 42.501928, 24.1867385);
        public readonly static Court Court524 = new Court("524", "Районен Съд Пещера", "Районен Съд", "Пещера", "4550", "ул. Васил Левски 2А", "0350 60151", "peshtera-rs@justice.bg", "https://peshtera-rs.justice.bg", 42.0341, 24.3037);
        public readonly static Court Court530 = new Court("530", "Окръжен Съд Пловдив", "Окръжен Съд", "Пловдив", "4000", "бул. 6 септември 167", "032 656231", "pvokrsad@dir.bg", "https://plovdiv-os.justice.bg", 42.151972, 24.7475685);
        public readonly static Court Court531 = new Court("531", "Районен Съд Асеновград", "Районен Съд", "Асеновград", "4230", "ул. Цар Иван Асен ІІ 6", "0331 62232", "info@court-asenovgrad.org", "https://asenovgrad-rs.justice.bg", 42.0048271, 24.8753559);
        public readonly static Court Court532 = new Court("532", "Районен Съд Карлово", "Районен Съд", "Карлово", "4300", "ул. Димитър Събев 4", "0335 94516", "krs@rs-karlovo.com", "https://karlovo-rs.justice.bg/", 42.640748, 24.8087185);
        
        public readonly static Court Court533 = new Court("533", "Районен Съд Пловдив", "Районен Съд", "Пловдив", "4000", "бул. 6 септември 167", "032 656362", "plovdiv-rs@justice.bg", "https://plovdiv-rs.justice.bg", 42.151972, 24.7475685);
        
        public readonly static Court Court534 = new Court("534", "Районен Съд Първомай", "Районен Съд", "Първомай", "4270", "ул. Христо Ботев 13", "0336 62261", "rsp@parvomai.escom.bg", "https://parvomay-rs.justice.bg", 42.099292, 25.225941);
        public readonly static Court Court540 = new Court("540", "Окръжен Съд Смолян", "Окръжен Съд", "Смолян", "4700", "бул. България 16", "0301 62812", "dc_sm@mail.bg", "https://smolyan-os.justice.bg", 41.5755669, 24.7112455);
        public readonly static Court Court541 = new Court("541", "Районен Съд Девин", "Районен Съд", "Девин", "4800", "ул. Ал. Костов 1", "03041 2022", "devin_rs@abv.bg", "https://devin-rs.justice.bg", 41.7429109, 24.3915387);
        public readonly static Court Court542 = new Court("542", "Районен Съд Златоград", "Районен Съд", "Златоград", "4980", "бул. България 120", "030712453", "rszlatograd@mbox.contact.bg", "https://zlatograd-rs.justice.bg", 41.3798446, 25.096521);
        public readonly static Court Court543 = new Court("543", "Районен Съд Мадан", "Районен Съд", "Мадан", "4900", "ул. Обединение 8", "0308 22117", "court_madan@mail.bg", "https://madan-rs.justice.bg", 41.5001879, 24.939005);
        public readonly static Court Court544 = new Court("544", "Районен Съд Смолян", "Районен Съд", "Смолян", "4700", "бул. България 16", "0301 60450", "rc_smolyan@mail.bg", "https://smolyan-rs.justice.bg", 41.5755669, 24.7112455);
        public readonly static Court Court545 = new Court("545", "Районен Съд Чепеларе", "Районен Съд", "Чепеларе", "4850", "ул. Беломорска 48", "03051 2127", "court_che@abv.bg", "https://chepelare-rs.justice.bg", 41.725176, 24.684670);
        public readonly static Court Court550 = new Court("550", "Окръжен Съд Стара Загора", "Окръжен Съд", "Стара Загора ", "6000", "бул.”Митрополит Методий Кусев” 33", "+359 42 220 262", "osstz@osstz.com", "https://stzagora-os.justice.bg", 42.4266137, 25.6252062);
        public readonly static Court Court551 = new Court("551", "Районен Съд Казанлък", "Районен Съд", "Казанлък", "6100", "ул. П. Хилендарски 16", "0431 67221", "krs@kz-court.org", "https://kazanlak-rs.justice.bg", 42.6207519, 25.3961278);
        public readonly static Court Court552 = new Court("552", "Районен Съд Раднево", "Районен Съд", "Раднево", "6260", "ул. Тачо Даскалов 1", "0417 83433", "rs_radnevo@abv.bg", "https://radnevo-rs.justice.bg", 42.28987, 25.933935);
        public readonly static Court Court553 = new Court("553", "Районен Съд Стара Загора", "Районен Съд", "Стара Загора", "6000", "бул. Митрополит Методи Кусев 33", "042 900847", "court@rs-sz.com", "https://stzagora-rs.justice.bg", 42.4266137, 25.6252062);
        public readonly static Court Court554 = new Court("554", "Районен Съд Чирпан", "Районен Съд", "Чирпан", "6200", "бул. Г. Димитров 28", "0416 92592", "as_rscir@abv.bg", "https://chirpan-rs.justice.bg", 42.1984524, 25.329173);
        public readonly static Court Court555 = new Court("555", "Районен Съд Гълъбово", "Районен Съд", "Гълъбово", "6280", "ул. Стефан Стамболов 2", "0418 65431", "rs_galabovo@mail.bg", "https://galabovo-rs.justice.bg", 42.135801, 25.864895);
        public readonly static Court Court560 = new Court("560", "Окръжен Съд Хасково", "Окръжен Съд", "Хасково", "6300", "бул. България 144", "038 601814", "sud@escom.bg", "https://haskovo-os.justice.bg", 41.9339688, 25.5500623);
        public readonly static Court Court561 = new Court("561", "Районен Съд Димитровград", "Районен Съд", "Димитровград", "6400", "бул. Г. С. Раковски 13", "0391 33170", "contacts@rs-dimitrovgrad.com", "https://dimitrovgrad-rs.justice.bg", 42.0608508, 25.5923712);
        public readonly static Court Court562 = new Court("562", "Районен Съд Свиленград", "Районен Съд", "Свиленград", "6500", "ул. Г. Бенковски 12", "0379 70600", "svilengrad-rs@justice.bg", "https://svilengrad-rs.justice.bg", 41.7692864, 26.1965022);
        public readonly static Court Court563 = new Court("563", "Районен Съд Харманли", "Районен Съд", "Харманли", "6450", "пл. Възраждане 11", "0373 82337", "rsharmanli@mail.bg", "https://harmanli-rs.justice.bg", 41.927521, 25.902472);
        public readonly static Court Court564 = new Court("564", "Районен Съд Хасково", "Районен Съд", "Хасково", "6300", "бул. България 144", "038 601905", "rshaskovo@mbox.contact.bg", "https://haskovo-rs.justice.bg", 41.9339688, 25.5500623);
        public readonly static Court Court600 = new Court("600", "Военно-апелативен съд София", "Военно-апелативен съд София", "София", "1000", "бул. Витоша 2", "02 9219617", "vasbg@dir.bg", "https://voas.justice.bg", 42.6954187, 23.3205029);
        public readonly static Court Court610 = new Court("610", "Военен Съд София", "Военен Съд", "София", "1000", "бул. Витоша 2", "02 9219498", "svs1990@abv.bg", "https://sofia-vs.justice.bg", 42.6954187, 23.3205029);
        public readonly static Court Court620 = new Court("620", "Военен Съд Пловдив", "Военен Съд", "Пловдив", "4000", "ул. Г. М. Димитров 28", "032 621242", "voenensd@gmail.com", "https://plovdiv-vs.justice.bg/", 42.1386708, 24.7474364);
        public readonly static Court Court630 = new Court("630", "Военен Съд Варна", "Военен Съд", "Варна", "9000", "бул. Владислав Варненчик 57", "052 612188", "vvs-adm@mbox.contact.bg", "http://vsvarna.court-bg.org", 43.211377, 27.9063091);
        public readonly static Court Court640 = new Court("640", "Военен Съд Плевен", "Военен Съд", "Плевен", "5800", "ул. Димитър Константинов 25", "064 822609", "", "", 43.4086943, 24.6208884);
        public readonly static Court Court650 = new Court("650", "Военен Съд Сливен", "Военен Съд", "Сливен", "8800", "бул. Г. Данчев 1", "044 667213", "voenensud@abv.bg", "https://sliven-vs.justice.bg", 42.678453, 26.3264363);
        public readonly static Court Court001 = new Court("001", "Върховен Административен Съд", "Върховен Административен Съд", "София", "1301", "бул. Александър Стамболийски 18", "02 9884902", "edocuments@sac.justice.bg", "www.sac.government.bg", 42.6971239, 23.3200898);
        public readonly static Court Court701 = new Court("701", "Административен съд София град", "Административен съд", "София", "1000", "ул. Георг Вашингтон 17", "02 42 15 724", "kabinet@admincourtsofia.bg", "https://sofia-adms-g.justice.bg", 42.69983, 23.320975);
        public readonly static Court Court702 = new Court("702", "Административен съд София област", "Административен съд", "София", "1000", "ул. Съборна 7", "02 8050711", "sda_court@abv.bg", "https://sofia-adms-o.justice.bg", 42.6960933, 23.3234609);
        public readonly static Court Court703 = new Court("703", "Административен съд Благоевград", "Административен съд", "Благоевград", "2700", "ул.Тодор Александров 47", "073 872684", "admcourtblg@bl-adc.justice.bg", "https://blagoevgrad-adms.justice.bg", 42.0170097, 23.0955635);
        public readonly static Court Court704 = new Court("704", "Административен съд Бургас", "Административен съд", "Бургас", "8000", "ул. Александровска 101", "056 810490", "ad_sad_bs@abv.bg", "https://burgas-adms.justice.bg", 42.498273, 27.470817);
        public readonly static Court Court705 = new Court("705", "Административен съд Варна", "Административен съд", "Варна", "9010", "ул. Никола Вапцаров 3А", "052 712805", "admcourt.vn@gmail.com", "https://varna-adms.justice.bg", 43.2130842, 27.934346);
        public readonly static Court Court706 = new Court("706", "Административен съд Велико Търново", "Административен съд", "Велико Търново", "5000", "пл. Център 2", "062 604102", "adm.asvt@abv.bg", "https://vtarnovo-adms.justice.bg", 43.0795106, 25.63567);
        public readonly static Court Court707 = new Court("707", "Административен съд Видин", "Административен съд", "Видин", "3700", "пл. Бдинци 1", "094 626363", "acvidin@acvidin.org", "https://vidin-adms.justice.bg", 43.9864407, 22.8772471);
        public readonly static Court Court708 = new Court("708", "Административен съд Враца", "Административен съд", "Враца", "3000", "ул. Иванка Ботева 16", "092 620606", "asvr@abv.bg", "https://vratsa-adms.justice.bg", 43.2065533, 23.5506788);
        public readonly static Court Court709 = new Court("709", "Административен съд Габрово", "Административен съд", "Габрово", "5300", "ул. Райчо Каролев 4", "066 810722", "gabrovo-adms@justice.bg", "https://gabrovo-adms.justice.bg", 42.8687123, 25.318453);
        public readonly static Court Court710 = new Court("710", "Административен съд Добрич", "Административен съд", "Добрич", "9300", "ул. Д-р Константин Стоилов 7", "058 622611", "asdobrich@mail.bg", "https://dobrich-adms.justice.bg", 43.5719477, 27.8293558);
        public readonly static Court Court711 = new Court("711", "Административен съд Кюстендил", "Административен съд", "Кюстендил", "2500", "ул. Гороцветна 29A", "078 551986", "adm.kas@mbox.contact.bg", "https://kyustendil-adms.justice.bg", 42.2781752, 22.6880401);
        public readonly static Court Court712 = new Court("712", "Административен съд Кърджали", "Административен съд", "Кърджали", "6600", "ул. Булаир 12", "0361 68000", "admsad_kj@abv.bg", "https://kardzhali-adms.justice.bg", 41.6384468, 25.3680148);
        public readonly static Court Court713 = new Court("713", "Административен съд Ловеч", "Административен съд", "Ловеч", "5500", "ул.Търговска 40", "068 601344", "as_lovech@abv.bg", "https://lovech-adms.justice.bg", 43.1377264, 24.7178821);
        public readonly static Court Court714 = new Court("714", "Административен съд Монтана", "Административен съд", "Монтана", "3400", "пл. Жеравица 3", "096 390812", "as.montana@mail.bg", "https://montana-adms.justice.bg", 43.4085367, 23.227404);
        public readonly static Court Court715 = new Court("715", "Административен съд Пазарджик", "Административен съд", "Пазарджик", "4400", "ул. Константин Величков 20", "034 407211", "admincourt-pz@admincourt-pz.bg", "https://pazardzhik-adms.justice.bg", 42.1913861, 24.3294637);
        public readonly static Court Court716 = new Court("716", "Административен съд Перник", "Административен съд", "Перник", "2300", "ул. Търговска 37", "076 647012", "ospernik@abv.bg", "https://pernik-adms.justice.bg", 42.6021866, 23.0299629);
        public readonly static Court Court717 = new Court("717", "Административен съд Плевен", "Административен съд", "Плевен", "5800", "ул. П. Р. Славейков 21", "064 899900", "mail@ac-pleven.org", "https://pleven-adms.justice.bg", 43.4163374, 24.6180077);
        public readonly static Court Court718 = new Court("718", "Административен съд Пловдив", "Административен съд", "Пловдив", "4000", "ул. Иван Вазов 20", "032 261064", "adms_pv@abv.bg", "https://plovdiv-adms.justice.bg/", 42.1400699, 24.7474788);
        public readonly static Court Court719 = new Court("719", "Административен съд Разград", "Административен съд", "Разград", "7200", "бул. Бели Лом 33", "+359 (84) 612 177", "delovodstvo@razgrad-adms.justice.bg", "https://razgrad-adms.justice.bg", 43.5249654, 26.528639);
        public readonly static Court Court720 = new Court("720", "Административен съд Русе", "Административен съд", "Русе", "7000", "ул. Цариброд 6", "082 511632", "as@admcourt-ruse.com", "https://ruse-adms.justice.bg", 43.8473028, 25.9499378);
        public readonly static Court Court721 = new Court("721", "Административен съд Силистра", "Административен съд", "Силистра", "7500", "ул. Цар Шишман 5", "086 822321", "adm_sad_silistra@abv.bg", "https://silistra-adms.justice.bg", 44.1178071, 27.2618731);
        
        public readonly static Court Court722 = new Court("722", "Административен съд Сливен", "Административен съд", "Сливен", "8800", "бул. Цар Освободител 12", "087 5317089", "delovodstvo@sliven-adms.justice.bg", "https://sliven-adms.justice.bg", 42.6805218, 26.3174013);
        
        public readonly static Court Court723 = new Court("723", "Административен съд Смолян", "Административен съд", "Смолян", "4700", "бул. България 16", "0301 81420", "ac_smolian@abv.bg", "https://smolyan-adms.justice.bg", 41.5755669, 24.7112455);
        public readonly static Court Court724 = new Court("724", "Административен съд Стара Загора", "Административен съд", "Стара Загора", "6000", "бул. Руски 44", " 042 651040", "info@adms-sz.com", "https://stzagora-adms.justice.bg", 42.4274205, 25.6276939);
        public readonly static Court Court725 = new Court("725", "Административен съд Търговище", "Административен съд", "Търговище", "7700", "пл. Свобода 1", "0601 62268", "as_tg@abv.bg", "https://targovishte-adms.justice.bg", 43.2461974, 26.5725975);
        public readonly static Court Court726 = new Court("726", "Административен съд Хасково", "Административен съд", "Хасково", "6300", "ул. Преслав 26", "038 626012", "has@admsudhaskovo.org", "https://haskovo-adms.justice.bg", 41.9312425, 25.5533571);
        public readonly static Court Court727 = new Court("727", "Административен съд Шумен", "Административен съд", "Шумен", "9700", "ул. Адам Мицкевич 1", "054 800791", "shumen-adms@justice.bg", "https://shumen-adms.justice.bg", 43.2701835, 26.9233979);
        public readonly static Court Court728 = new Court("728", "Административен съд Ямбол", "Административен съд", "Ямбол", "8600", "ул. Александър Стамболийски 4", "046 681000", "adms_yambol@yambollan.com", "https://yambol-adms.justice.bg", 42.4851561, 26.5049071);
        public readonly static Court Court002 = new Court("002", "Върховен Касационен Съд", "Върховен Касационен Съд", "София", "1000", "бул. Витоша 2", "02 921988", "http://www.vks.bg/vks_p03.htm", "www.vks.bg", 42.6954187, 23.3205029);

        public readonly static List<Court> CourtsList = new List<Court>()
        {
            Court100,
            Court101,
            Court105,
            Court110,
            Court1111,
            Court1112,
            Court120,
            Court121,
            Court122,
            Court123,
            Court124,
            Court125,
            Court130,
            Court131,
            Court132,
            Court133,
            Court140,
            Court141,
            Court142,
            Court143,
            Court144,
            Court145,
            Court146,
            Court150,
            Court151,
            Court152,
            Court160,
            Court161,
            Court162,
            Court163,
            Court170,
            Court171,
            Court172,
            Court173,
            Court174,
            Court180,
            Court181,
            Court182,
            Court183,
            Court184,
            Court185,
            Court186,
            Court187,
            Court188,
            Court189,
            Court200,
            Court210,
            Court211,
            Court212,
            Court213,
            Court214,
            Court215,
            Court216,
            Court217,
            Court218,
            Court220,
            Court221,
            Court222,
            Court223,
            Court230,
            Court231,
            Court232,
            Court233,
            Court300,
            Court310,
            Court311,
            Court312,
            Court313,
            Court320,
            Court321,
            Court322,
            Court323,
            Court324,
            Court325,
            Court330,
            Court331,
            Court332,
            Court333,
            Court340,
            Court341,
            Court342,
            Court343,
            Court350,
            Court351,
            Court352,
            Court353,
            Court360,
            Court361,
            Court362,
            Court363,
            Court400,
            Court410,
            Court411,
            Court412,
            Court413,
            Court414,
            Court415,
            Court420,
            Court421,
            Court422,
            Court423,
            Court424,
            Court430,
            Court431,
            Court432,
            Court433,
            Court434,
            Court440,
            Court441,
            Court442,
            Court443,
            Court444,
            Court450,
            Court451,
            Court452,
            Court500,
            Court510,
            Court511,
            Court512,
            Court513,
            Court514,
            Court515,
            Court520,
            Court521,
            Court522,
            Court523,
            Court524,
            Court530,
            Court531,
            Court532,
            Court533,
            Court534,
            Court540,
            Court541,
            Court542,
            Court543,
            Court544,
            Court545,
            Court550,
            Court551,
            Court552,
            Court553,
            Court554,
            Court555,
            Court560,
            Court561,
            Court562,
            Court563,
            Court564,
            Court600,
            Court610,
            Court620,
            Court630,
            Court640,
            Court650,
            Court001,
            Court701,
            Court702,
            Court703,
            Court704,
            Court705,
            Court706,
            Court707,
            Court708,
            Court709,
            Court710,
            Court711,
            Court712,
            Court713,
            Court714,
            Court715,
            Court716,
            Court717,
            Court718,
            Court719,
            Court720,
            Court721,
            Court722,
            Court723,
            Court724,
            Court725,
            Court726,
            Court727,
            Court728,
            Court002
        };
    }

    public class Court
    {
        public Court(
        string code,
        string name,
        string type,
        string city,
        string postcode,
        string address,
        string phone,
        string mail,
        string website,
        double latitude,
        double longitude)
        {
            this.CourtCode = code;
            this.Name = name;
            this.CourtType = type;
            this.City = city;
            this.Postcode = postcode;
            this.Address = address;
            this.Phone = phone;
            this.Email = mail;
            this.Website = website;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public string CourtCode { get; set; }

        public string Name { get; set; }

        public string CourtType { get; set; }

        public string City { get; set; }

        public string Postcode { get; set; }

        public string Address { get; set; }

        public string AddressNumber { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string CityAddress
        {
            get
            {
                return String.Join(", ", new List<string>() { this.City, this.Postcode }.Where(e => !String.IsNullOrWhiteSpace(e)));
            }
        }

        public string StreetAddress
        {
            get
            {
                return String.Join(", ", new List<string>() { this.Address, this.AddressNumber }.Where(e => !String.IsNullOrWhiteSpace(e)));
            }
        }

        public string HttpWebsite
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.Website) && !this.Website.StartsWith("http"))
                    return "http://" + this.Website;
                else
                    return this.Website;
            }
        }

        public string Tooltip
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<p>");
                if (!String.IsNullOrWhiteSpace(this.HttpWebsite))
                    sb.Append(String.Format("<a href=\"{0}\" target=\"_blank\">", this.HttpWebsite));

                sb.Append(String.Format("<strong>{0}</strong>", this.Name));

                if (!String.IsNullOrWhiteSpace(this.HttpWebsite))
                    sb.Append("</a>");
                sb.Append("</p>");

                if (!String.IsNullOrWhiteSpace(this.CityAddress))
                    sb.Append(String.Format("<p>{0}</p>", this.CityAddress));

                if (!String.IsNullOrWhiteSpace(this.StreetAddress))
                    sb.Append(String.Format("<p>{0}</p>", this.StreetAddress));

                if (!String.IsNullOrWhiteSpace(this.Phone))
                    sb.Append(String.Format("<p>{0}</p>", this.Phone));

                if (!String.IsNullOrWhiteSpace(this.Email))
                    sb.Append(String.Format("<a href=\"mailto:{0}\">{0}</a>", this.Email));

                return sb.ToString();
            }
        }

        public IEnumerable<object> ToObjectList()
        {
            return new List<object>()
            {
                this.CourtCode,
                this.Tooltip,
                this.Latitude,
                this.Longitude,
                this.CourtCode,
                Links.Content.img.markers.marker01_png,
                this.Name
            };
        }
    }
}
