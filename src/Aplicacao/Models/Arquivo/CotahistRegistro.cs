using FileHelpers;
using System.Globalization;

namespace Aplicacao.Models.Arquivo
{
    [FixedLengthRecord]
    [IgnoreFirst(1)]
    [IgnoreLast(1)]
    public class CotahistRegistro
    {
        [FieldFixedLength(2)]
        public string TIPREG; // 01 a 02

        [FieldFixedLength(8)]
        [FieldConverter(typeof(DataPregaoConverter))]
        public DateTime DATPRE; // 03 a 10

        [FieldFixedLength(2)]
        public string CODBDI; // 11 a 12

        [FieldFixedLength(12)]
        [FieldTrim(TrimMode.Both)]
        public string CODNEG; // 13 a 24

        [FieldFixedLength(3)]
        public string TPMERC; // 25 a 27

        [FieldFixedLength(12)]
        [FieldTrim(TrimMode.Both)]
        public string NOMRES; // 28 a 39

        [FieldFixedLength(10)]
        [FieldTrim(TrimMode.Both)]
        public string ESPECI; // 40 a 49

        [FieldFixedLength(3)]
        public string PRAZOT; // 50 a 52

        [FieldFixedLength(4)]
        public string MODREF; // 53 a 56

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREABE; // 57 a 69

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREMAX; // 70 a 82

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREMIN; // 83 a 95

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREMED; // 96 a 108

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREULT; // 109 a 121

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREOFC; // 122 a 134

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREOFV; // 135 a 147

        [FieldFixedLength(5)]
        public int TOTNEG; // 148 a 152

        [FieldFixedLength(18)]
        public long QUATOT; // 153 a 170

        [FieldFixedLength(18)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal VOLTOT; // 171 a 188

        [FieldFixedLength(13)]
        [FieldConverter(typeof(PrecoB3Converter))]
        public decimal PREEXE; // 189 a 201

        [FieldFixedLength(1)]
        public string INDOPC; // 202 a 202

        [FieldFixedLength(8)]
        public string DATVEN; // 203 a 210 

        [FieldFixedLength(7)]
        public int FATCOT; // 211 a 217

        [FieldFixedLength(13)]
        public string PTOEXE; // 218 a 230

        [FieldFixedLength(12)]
        public string CODISI; // 231 a 242

        [FieldFixedLength(3)]
        public int DISMES; // 243 a 245

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
