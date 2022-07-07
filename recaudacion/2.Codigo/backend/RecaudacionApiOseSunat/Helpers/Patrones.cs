namespace RecaudacionApiOseSunat.Helpers
{
    public class FabricaSingleton<T> where T : new()
    {
        static T _t;

        public static T Crear()
        {
            if (_t == null)
            {
                _t = new T();
            }

            return _t;
        }
    }
}
