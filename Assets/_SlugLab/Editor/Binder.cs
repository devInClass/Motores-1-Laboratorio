using System;
using System.Reflection;
using UnityEngine;

namespace SlugLab.EditorTools
{
    /// <summary>
    /// Asigna campos privados [SerializeField] por reflexión. Se usa SOLO en el
    /// generador de escenas, para no tener que hacer público lo que debe ser
    /// privado en el juego.
    /// </summary>
    public static class Binder
    {
        public static void Set(object target, string fieldName, object value)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            Type type = target.GetType();
            FieldInfo field = null;

            while (type != null && field == null)
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                type = type.BaseType;
            }

            if (field == null)
                throw new MissingFieldException($"[Binder] El campo '{fieldName}' no existe en {target.GetType().Name}");

            field.SetValue(target, value);
        }

        public static void SetMask(object target, string fieldName, params string[] layerNames)
        {
            Set(target, fieldName, (LayerMask)LayerMask.GetMask(layerNames));
        }

        /// <summary>Asigna una referencia a un componente concreto (evita ambigüedad de tipos).</summary>
        public static void SetRef(object target, string fieldName, UnityEngine.Object reference)
        {
            Set(target, fieldName, reference);
        }
    }
}
