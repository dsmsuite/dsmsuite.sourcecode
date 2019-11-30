namespace DsmSuite.Transformer.Transformation
{
    public abstract class Action
    {
        protected Action(string name, bool enabled)
        {
            Name = name;
            IsEnabled = enabled;
        }

        public bool IsEnabled { get; }

        public void Execute()
        {
            if (IsEnabled)
            {
                ExecuteImpl();
            }
        }

        protected abstract void ExecuteImpl();

        public string Name { get; }
    }
}
