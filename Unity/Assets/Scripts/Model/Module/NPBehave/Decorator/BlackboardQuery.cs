namespace NPBehave
{
    public class BlackboardQuery : ObservingDecorator
    {
        private string[]          keys;
        private System.Func<bool> query;

        public BlackboardQuery(string[] keys, Stops stopsOnChange, System.Func<bool> query, Node decoratee) : base("BlackboardQuery", stopsOnChange, decoratee)
        {
            this.keys = keys;
            this.query = query;
        }

        override protected void StartObserving()
        {
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                this.RootNode.Blackboard.AddObserver(keys[i], onValueChanged);
            }
        }

        override protected void StopObserving()
        {
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                this.RootNode.Blackboard.RemoveObserver(keys[i], onValueChanged);
            }
        }

        private void onValueChanged(Blackboard.Type type, object newValue)
        {
            Evaluate();
        }

        protected override bool IsConditionMet()
        {
            return this.query();
        }

        override public string ToString()
        {
            string keyss = "";

            for (int i = 0; i < keys.Length; i++)
            {
                keyss += " " + keys[i];
            }

            return Name + keyss;
        }
    }
}