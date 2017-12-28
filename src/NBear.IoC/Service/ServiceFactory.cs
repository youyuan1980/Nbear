using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Castle.Windsor;
using System.Reflection.Emit;
using System.Configuration;
using NBear.Common;
using NBear.Net.Remoting;

namespace NBear.IoC.Service
{
    /// <summary>
    /// The service factory.
    /// </summary>
    public class ServiceFactory : ILogable
    {
        #region Emit DynamicServiceImpl

        private object syncObj = new object();
        private static AssemblyBuilder assBuilder = null;
        private static ModuleBuilder modBuilder = null;

        private const string DYNAMIC_INTERFACEIMPL_NAMESPACE = "NBear.IoC.Service.DynamicInterfaceImpl";

        private static ResolveEventHandler _ResolveEventHandler = null;

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return assBuilder;
        }

        private static void EmitConstructor<IServiceInterfaceType>(TypeBuilder typeBuilder, Type baseType)
            //where IServiceInterfaceType : IServiceInterface
        {
            //define default constructor
            ConstructorBuilder consBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(IServiceContainer) });
            ILGenerator ctorIL = consBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Ldtoken, typeof(IServiceInterfaceType));
            ctorIL.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public));
            ctorIL.Emit(OpCodes.Call, baseType.GetConstructor(new Type[] { typeof(IServiceContainer), typeof(Type) }));
            ctorIL.Emit(OpCodes.Ret);
        }

        private static void EmitLoadInt32Value(ILGenerator methodIL, int int32Value)
        {
            Check.Require(methodIL != null);

            if (int32Value == 0)
            {
                methodIL.Emit(OpCodes.Ldc_I4_0);
            }
            else if (int32Value == 1)
            {
                methodIL.Emit(OpCodes.Ldc_I4_1);
            }
            else if (int32Value == 2)
            {
                methodIL.Emit(OpCodes.Ldc_I4_2);
            }
            else if (int32Value == 3)
            {
                methodIL.Emit(OpCodes.Ldc_I4_3);
            }
            else if (int32Value == 4)
            {
                methodIL.Emit(OpCodes.Ldc_I4_4);
            }
            else if (int32Value == 5)
            {
                methodIL.Emit(OpCodes.Ldc_I4_5);
            }
            else if (int32Value == 6)
            {
                methodIL.Emit(OpCodes.Ldc_I4_6);
            }
            else if (int32Value == 7)
            {
                methodIL.Emit(OpCodes.Ldc_I4_7);
            }
            else if (int32Value == 8)
            {
                methodIL.Emit(OpCodes.Ldc_I4_8);
            }
            else
            {
                methodIL.Emit(OpCodes.Ldc_I4_S, int32Value);
            }
        }

        private static void EmitMethods<IServiceInterfaceType>(TypeBuilder typeBuilder, Type baseType, MethodInfo[] mis)
            //where IServiceInterfaceType : IServiceInterface
        {
            if (mis != null && mis.Length > 0)
            {
                foreach (MethodInfo mi in mis)
                {
                    ParameterInfo[] paramInfos = mi.GetParameters();
                    int paramlength = paramInfos.Length;
                    Type[] paramTypes = new Type[paramlength];
                    for (int i = 0; i < paramlength; i++)
                    {
                        paramTypes[i] = paramInfos[i].ParameterType;
                    }
                    MethodBuilder methodBuilder = typeBuilder.DefineMethod(mi.Name, mi.Attributes & (~MethodAttributes.Abstract) | MethodAttributes.Public, mi.CallingConvention, mi.ReturnType, paramTypes);
                    for (int i = 0; i < paramlength; i++)
                    {
                        ParameterInfo pi = paramInfos[i];
                        methodBuilder.DefineParameter(i + 1, pi.Attributes, pi.Name);
                    }
                    typeBuilder.DefineMethodOverride(methodBuilder, mi);
                    ILGenerator methodIL = methodBuilder.GetILGenerator();

                    if (paramlength > 0)
                    {
                        methodIL.DeclareLocal(typeof(object[]));
                    }

                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldstr, mi.ToString());
                    methodIL.Emit(OpCodes.Ldtoken, mi.ReturnType);
                    methodIL.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public));
                    EmitLoadInt32Value(methodIL, paramlength);
                    methodIL.Emit(OpCodes.Newarr, typeof(object));

                    if (paramlength > 0)
                    {
                        methodIL.Emit(OpCodes.Stloc_0);
                        for (int i = 0; i < paramlength; i++)
                        {
                            methodIL.Emit(OpCodes.Ldloc_0);
                            EmitLoadInt32Value(methodIL, i);

                            switch (i)
                            {
                                case 0:
                                    methodIL.Emit(OpCodes.Ldarg_1);
                                    break;
                                case 1:
                                    methodIL.Emit(OpCodes.Ldarg_2);
                                    break;
                                case 2:
                                    methodIL.Emit(OpCodes.Ldarg_3);
                                    break;
                                default:
                                    methodIL.Emit(OpCodes.Ldarg_S, i + 1);
                                    break;
                            }

                            if (paramInfos[i].ParameterType.IsValueType)
                            {
                                methodIL.Emit(OpCodes.Box, paramInfos[i].ParameterType);
                            }

                            methodIL.Emit(OpCodes.Stelem_Ref);
                        }
                        methodIL.Emit(OpCodes.Ldloc_0);
                    }
                    methodIL.Emit(OpCodes.Callvirt, baseType.GetMethod("CallService", BindingFlags.Instance | BindingFlags.NonPublic));
                    if (mi.ReturnType == typeof(void))
                    {
                        methodIL.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        if (mi.ReturnType.IsValueType)
                        {
                            methodIL.Emit(OpCodes.Unbox_Any, mi.ReturnType);
                        }
                        else
                        {
                            methodIL.Emit(OpCodes.Castclass, mi.ReturnType);
                        }
                    }
                    methodIL.Emit(OpCodes.Ret);
                }
            }
        }

        private IServiceInterfaceType EmitDynamicServiceInterfaceImplType<IServiceInterfaceType>(AssemblyBuilder assBuilder, ModuleBuilder modBuilder)
            //where IServiceInterfaceType : IServiceInterface
        {
            TypeBuilder typeBuilder = modBuilder.DefineType(DYNAMIC_INTERFACEIMPL_NAMESPACE + "." + typeof(IServiceInterfaceType).FullName, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IServiceInterfaceType));
            MethodInfo[] mis = typeof(IServiceInterfaceType).GetMethods();
            List<MethodInfo> miList = new List<MethodInfo>();
            foreach (MethodInfo mi in mis)
            {
                miList.Add(mi);
            }
            foreach (Type inheritedInterface in typeof(IServiceInterfaceType).GetInterfaces())
            {
                typeBuilder.AddInterfaceImplementation(inheritedInterface);
                foreach (MethodInfo mi in inheritedInterface.GetMethods())
                {
                    miList.Add(mi);
                }
            }
            Type baseType = typeof(BaseServiceInterfaceImpl);
            typeBuilder.SetParent(baseType);

            EmitConstructor<IServiceInterfaceType>(typeBuilder, baseType);

            EmitMethods<IServiceInterfaceType>(typeBuilder, baseType, miList.ToArray());

            return (IServiceInterfaceType)Activator.CreateInstance(typeBuilder.CreateType(), container);
        }

        private IServiceInterfaceType DynamicServiceImpl<IServiceInterfaceType>()
            //where IServiceInterfaceType : IServiceInterface
        {
            Type t = null;
            if (assBuilder != null)
            {
                t = assBuilder.GetType(DYNAMIC_INTERFACEIMPL_NAMESPACE + "." + typeof(IServiceInterfaceType).FullName);
            }
            if (t != null)
            {
                return (IServiceInterfaceType)Activator.CreateInstance(t, container);
            }

            lock(syncObj)
            {
                //create dynamic IEntityType Assembly & Type through Emit
                if (assBuilder == null)
                {
                    AssemblyName assName = new AssemblyName();
                    assName.Name = DYNAMIC_INTERFACEIMPL_NAMESPACE;
                    assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);

                    //add dynamic assembly to current appdomain
                    if (_ResolveEventHandler == null)
                    {
                        _ResolveEventHandler = new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                        AppDomain.CurrentDomain.AssemblyResolve += _ResolveEventHandler;
                    }
                }

                if (modBuilder == null)
                {
                    modBuilder = assBuilder.DefineDynamicModule(assBuilder.GetName().Name);
                }

                return EmitDynamicServiceInterfaceImplType<IServiceInterfaceType>(assBuilder, modBuilder);
            }
        }

        #endregion

        #region Create Service Factory

        private IServiceContainer container;

        /// <summary>
        /// Gets the service container.
        /// </summary>
        /// <value>The service container.</value>
        public IServiceContainer ServiceContainer
        {
            get
            {
                return container;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFactory"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        protected ServiceFactory(IServiceContainer container)
        {
            if (container == null)
            {
                this.container = new SimpleServiceContainer();
            }
            else
            {
                this.container = container;
            }

            this.container.OnLog += new LogHandler(WriteLog);
        }

        private void WriteLog(string logInfo)
        {
            if (OnLog != null) OnLog(logInfo);
        }

        /// <summary>
        /// Loads the service factory configuration.
        /// </summary>
        /// <returns>The section.</returns>
        public static Configuration.ServiceFactoryConfigurationSection LoadServiceFactoryConfiguration()
        {
            object config = ConfigurationManager.GetSection("serviceFactory");
            if (config == null)
            {
                return new Configuration.ServiceFactoryConfigurationSection();
            }
            return (Configuration.ServiceFactoryConfigurationSection)config;
        }

        private static ServiceFactory singleton = null;
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>The service factoru singleton instance.</returns>
        public static ServiceFactory Create()
        {
            if (singleton == null)
            {
                Configuration.ServiceFactoryConfigurationSection config = LoadServiceFactoryConfiguration();

                if (config.Type == NBear.IoC.Service.Configuration.ServiceFactoryType.Local)
                {
                    singleton = new ServiceFactory(new SimpleServiceContainer());
                }
                else
                {
                    RemotingClientHelper helper = new RemotingClientHelper(config.Protocol, config.Server, config.Port, 0);
                    IServiceMQ mq = helper.GetWellKnownClientInstance<IServiceMQ>(config.ServiceMQName);
                    singleton = new ServiceFactory(new SimpleServiceContainer(mq));
                }
                singleton.ServiceContainer.Compress = config.Compress;
                singleton.ServiceContainer.MaxTryNum = config.MaxTry;
            }

            return singleton;
        }

        #endregion

        #region Get Service

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <returns>The service implemetation instance.</returns>
        public IServiceInterfaceType GetService<IServiceInterfaceType>()
            //where IServiceInterfaceType : IServiceInterface
        {
            Exception ex = new ArgumentException("Generic parameter type - IServiceInterfaceType must be an interface implementing NBear.IoC.Service.IServiceInterface or marked with ServiceContractAttribute.");
            if (!typeof(IServiceInterfaceType).IsInterface)
            {
                throw ex;
            }
            else if (!typeof(IServiceInterface).IsAssignableFrom(typeof(IServiceInterfaceType)))
            {
                bool markedWithServiceContract = false;
                foreach (object attr in typeof(IServiceInterfaceType).GetCustomAttributes(true))
                {
                    if (attr.ToString().EndsWith("ServiceContractAttribute"))
                    {
                        markedWithServiceContract = true;
                        break;
                    }
                }
                if (!markedWithServiceContract)
                {
                    throw ex;
                }
            }

            if (container.Kernel.HasComponent(typeof(IServiceInterfaceType)))
            {
                return (IServiceInterfaceType)container[typeof(IServiceInterfaceType)];
            }

            lock (this)
            {
                if (container != null)
                {
                    return DynamicServiceImpl<IServiceInterfaceType>();
                }
            }

            return default(IServiceInterfaceType);
        }

        #endregion

        #region ILogable Members

        /// <summary>
        /// OnLog event.
        /// </summary>
        public event LogHandler OnLog;

        #endregion
    }
}
