using System.Collections.Generic;

namespace Model
{
    public enum NumericType
    {
        Max       = 100000,
        THRESHOLD = 10000,

        Speed         = 1000,
        SpeedBase     = Speed * 10 + 1,
        SpeedAdd      = Speed * 10 + 2,
        SpeedPct      = Speed * 10 + 3,
        SpeedFinalAdd = Speed * 10 + 4,
        SpeedFinalPct = Speed * 10 + 5,

        Hp     = 1001,
        HpBase = Hp * 10 + 1,

        MaxHp         = 1002,
        MaxHpBase     = MaxHp * 10 + 1,
        MaxHpAdd      = MaxHp * 10 + 2,
        MaxHpPct      = MaxHp * 10 + 3,
        MaxHpFinalAdd = MaxHp * 10 + 4,
        MaxHpFinalPct = MaxHp * 10 + 5,

        Level     = 1003,
        LevelBase = Level * 10 + 1,
    }

    [LifeCycle]
    [ComponentOf(typeof(UnitComponent))]
    public class NumericComponent : Component, IAwake
    {
        private Dictionary<int, int> NumericDic;

        public void Awake()
        {
            NumericDic = new Dictionary<int, int>();
        }

        public override void Dispose()
        {
            NumericDic = null;

            base.Dispose();
        }

        public float GetAsFloat(NumericType numericType)
        {
            return (float)GetByKey((int)numericType) / 10000;
        }

        public int GetAsInt(NumericType numericType)
        {
            return GetByKey((int)numericType);
        }

        public void Set(NumericType nt, float value)
        {
            this[nt] = (int)(value * 10000);
        }

        public void Set(NumericType nt, int value)
        {
            this[nt] = value;
        }

        public int this[NumericType numericType]
        {
            get { return this.GetByKey((int)numericType); }
            set
            {
                int v = this.GetByKey((int)numericType);

                if (v == value)
                {
                    return;
                }

                NumericDic[(int)numericType] = value;

                if (numericType > NumericType.THRESHOLD)
                {
                    Update(numericType);
                }
            }
        }

        private int GetByKey(int key)
        {
            int value = 0;
            this.NumericDic.TryGetValue(key, out value);

            return value;
        }

        public void Update(NumericType numericType)
        {
            if (numericType > NumericType.Max)
            {
                return;
            }

            NumericType final = (NumericType)((int)numericType / 10);

            if (final == NumericType.Hp)
            {
                this[final] = this.GetAsInt(NumericType.HpBase);
            }
            else if (final == NumericType.Speed)
            {
                this[final] = this.GetAsInt(NumericType.SpeedBase);
            }
            else if (final == NumericType.MaxHp)
            {
                this[final] = this.GetAsInt(NumericType.MaxHpBase);
            }
            else if (final == NumericType.Level)
            {
                this[final] = this.GetAsInt(NumericType.LevelBase);
            }
        }
    }
}