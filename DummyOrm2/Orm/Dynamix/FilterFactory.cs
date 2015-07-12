using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DummyOrm2.Orm.Dynamix
{
    public static class FilterFactory
    {
        private static readonly ModuleBuilder ModuleBuilder;

        static FilterFactory()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("MyORM.Core.Dynamix.Pocos"),
                AssemblyBuilderAccess.Run);

            var assemblyName = assemblyBuilder.GetName().Name;

            ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        public static T CreateFilter<T>()
        {
            var typeName = typeof(T).Name + "Filter";

            var typeBuilder = ModuleBuilder.DefineType(
                "DummyOrm2.Dynamix.Filters." + typeName,
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(T),
                new[] { typeof(IFilter<T>) });

            // FilterValues _filter;
            var filter = typeBuilder.DefineField("_filter", typeof(IFilterValues), FieldAttributes.Private);

            // Ctor
            var baseCtor = typeof(T).GetConstructor(Type.EmptyTypes);

            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                Type.EmptyTypes);

            var ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, baseCtor);

            var filterCtor = typeof(FilterValues).GetConstructor(Type.EmptyTypes);

            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Newobj, filterCtor);
            ctorIL.Emit(OpCodes.Stfld, filter);
            ctorIL.Emit(OpCodes.Ret);

            // Implement interface
            var methodBuilder = typeBuilder.DefineMethod("GetValues",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                typeof(FilterValues),
                Type.EmptyTypes);

            var il = methodBuilder.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, filter);
            il.Emit(OpCodes.Ret);

            // Override properties 
            var properties = typeof(T).GetProperties();

            var setValueMethod = typeof(FilterValues).GetMethod("SetValue");

            foreach (var baseProp in properties)
            {
                var overrideProp = typeBuilder.DefineProperty(baseProp.Name, baseProp.Attributes, baseProp.PropertyType, null);
                
                //Getter
                var baseGetter = baseProp.GetGetMethod();

                var getterMethod = typeBuilder.DefineMethod(
                    baseGetter.Name,
                    MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    baseGetter.ReturnType,
                    Type.EmptyTypes);

                var getterIL = getterMethod.GetILGenerator();

                getterIL.Emit(OpCodes.Ldarg_0);
                getterIL.Emit(OpCodes.Call, baseGetter);
                getterIL.Emit(OpCodes.Ret);

                //Setter
                var baseSetter = baseProp.GetSetMethod();

                var setterMethod = typeBuilder.DefineMethod(
                    baseSetter.Name,
                    MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null,
                    new[] { baseSetter.GetParameters()[0].ParameterType });

                var setterIL = setterMethod.GetILGenerator();

                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Call, baseSetter);

                setterIL.Emit(OpCodes.Ldarg_0);
                setterIL.Emit(OpCodes.Ldfld, filter);
                setterIL.Emit(OpCodes.Ldstr, baseProp.Name);
                setterIL.Emit(OpCodes.Ldarg_1);
                setterIL.Emit(OpCodes.Call, setValueMethod);

                setterIL.Emit(OpCodes.Ret);

                overrideProp.SetGetMethod(getterMethod);
                overrideProp.SetSetMethod(setterMethod);
            }

            var type = typeBuilder.CreateType();
            return (T)Activator.CreateInstance(type);
        }
    }
}
