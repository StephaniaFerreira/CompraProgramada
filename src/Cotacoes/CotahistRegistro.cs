using FileHelpers;
using System.Globalization;

namespace Cotacoes
{
    [FixedLengthRecord]
    [IgnoreFirst(1)]
    [IgnoreLast(1)]
    public class CotahistRegistro
    {
        [FieldFixedLength(2)]
        public string TIPREG = null!;

        [FieldFixedLength(8)]
        [FieldConverter(typeof(DataPregaoConverter))]
        public DateTime DATPRE;

        [FieldFixedLength(2)]
        public string CODBDI = null!;

        [FieldFixedLength(12)]
        [FieldTrim(TrimMode.Both)]
        public string CODNEG = null!;

        [FieldFixedLength(3)]
        public string TPMERC = null!;

        [FieldFixedLength(30)]
        [FieldHidden]
        public string FILLER1 = null!;

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREABE;

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREMAX;

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREMIN;

        [FieldFixedLength(13)]
        [FieldHidden]
        public string FILLER2 = null!;

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREULT;
    }

    public class DataPregaoConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            return DateTime.ParseExact(
                from,
                "yyyyMMdd",
                CultureInfo.InvariantCulture);
        }
    }

    public class PrecoB3Converter : ConverterBase
    {
        public override object StringToField(string from)
        {
            if (string.IsNullOrWhiteSpace(from))
                return 0m;

            return decimal.Parse(from, CultureInfo.InvariantCulture) / 100m;
        }
    }
}
