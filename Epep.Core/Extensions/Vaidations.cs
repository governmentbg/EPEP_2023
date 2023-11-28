namespace Epep.Core.Extensions
{
    public static class Vaidations
    {
        public static bool IsValidPersonIdentifier(string uic)
        {
            return IsEGN(uic) || IsLnch(uic);
        }

        /// <summary>
        /// Check value is EGN
        /// </summary>
        /// <param name="EGN">string</param>
        /// <returns>string</returns>
        public static bool IsEGN(string EGN, bool InitiallyValidation = false)
        {
            if (EGN == null) return false;
            if (EGN.Length != 10) return false;
            if (EGN == "0000000000") return false;

            // само първична валидация
            if (InitiallyValidation)
            {
                decimal egn = 0;
                if (!decimal.TryParse(EGN, out egn)) return false;
                return true;
            }

            // пълна валидация
            int a = 0;
            int valEgn = 0;
            for (int i = 0; i < 10; i++)
            {
                if (!int.TryParse(EGN.Substring(i, 1), out a)) return false;
                switch (i)
                {
                    case 0:
                        valEgn += 2 * a;
                        continue;
                    case 1:
                        valEgn += 4 * a;
                        continue;
                    case 2:
                        valEgn += 8 * a;
                        continue;
                    case 3:
                        valEgn += 5 * a;
                        continue;
                    case 4:
                        valEgn += 10 * a;
                        continue;
                    case 5:
                        valEgn += 9 * a;
                        continue;
                    case 6:
                        valEgn += 7 * a;
                        continue;
                    case 7:
                        valEgn += 3 * a;
                        continue;
                    case 8:
                        valEgn += 6 * a;
                        continue;
                }
            }
            long chkSum = valEgn % 11;
            if (chkSum == 10)
                chkSum = 0;
            if (chkSum != Convert.ToInt64(EGN.Substring(9, 1))) return false;
            if ((int.Parse(EGN.Substring(8, 1)) / 2) == 0)
            {
                // girl person
                return true;
            }
            // guy person
            return true;
        }

        /// <summary>
        /// EIK validation
        /// </summary>
        /// <param name="EIK">string</param>
        /// <returns>bool</returns>
        public static bool IsEIK(string EIK)
        {
            if (EIK == null) return false;
            if ((EIK.Length != 9) && (EIK.Length != 13)) return false;

            int sum = 0, a = 0, chkSum = 0;
            for (int i = 0; i < 8; i++)
            {
                if (!int.TryParse(EIK.Substring(i, 1), out a)) return false;
                sum += a * (i + 1);
            }
            chkSum = sum % 11;
            if (chkSum == 10)
            {

                sum = 0;
                a = 0;
                chkSum = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (!int.TryParse(EIK.Substring(i, 1), out a)) return false;
                    sum += a * (i + 3);
                }
                chkSum = sum % 11;
                if (chkSum == 10) chkSum = 0;
            }

            if (chkSum.ToString() == EIK.Substring(8, 1))
            {
                if (EIK.Length == 9)
                {
                    return true;
                }
                else
                {
                    for (int i = 8; i < 12; i++)
                    {
                        if (!int.TryParse(EIK.Substring(i, 1), out a)) return false;
                        switch (i)
                        {
                            case 8:
                                sum = a * 2;
                                continue;
                            case 9:
                                sum += a * 7;
                                continue;
                            case 10:
                                sum += a * 3;
                                continue;
                            case 11:
                                sum += a * 5;
                                continue;
                        }
                    }
                    chkSum = sum % 11;
                    if (chkSum == 10)
                    {
                        for (int i = 8; i < 12; i++)
                        {
                            if (!int.TryParse(EIK.Substring(i, 1), out a)) return false;
                            switch (i)
                            {
                                case 8:
                                    sum = a * 4;
                                    continue;
                                case 9:
                                    sum += a * 9;
                                    continue;
                                case 10:
                                    sum += a * 5;
                                    continue;
                                case 11:
                                    sum += a * 7;
                                    continue;
                            }
                        }
                        chkSum = sum % 11;
                        if (chkSum == 10) chkSum = 0;
                        if (chkSum.ToString() == EIK.Substring(12, 1))
                            return true;
                        else
                            return false;
                    }
                }
            }
            else
                return false;

            return true;
        }

        public static bool IsLnch(string lnch)
        {
            if (string.IsNullOrEmpty(lnch))
            {
                return false;
            }

            if (lnch.Length != 10)
            {
                return false;
            }

            if (lnch.Any(c => !Char.IsDigit(c)))
            {
                return false;
            }

            return true;
        }
    }
}
