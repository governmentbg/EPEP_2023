using System.Collections.Generic;

namespace eCase.Common.Enums
{
    public class MessageSummonTypeNomenclature
    {
        public string Text { get; set; }

        public string Code { get; set; }

        public static readonly MessageSummonTypeNomenclature Message = new MessageSummonTypeNomenclature { Text = "Съобщение", Code = "1" };
        public static readonly MessageSummonTypeNomenclature Summon = new MessageSummonTypeNomenclature { Text = "Призовка", Code = "2" };

        public static readonly IEnumerable<MessageSummonTypeNomenclature> Values =
            new List<MessageSummonTypeNomenclature>
            {
                Message,
                Summon
            };
    }
}