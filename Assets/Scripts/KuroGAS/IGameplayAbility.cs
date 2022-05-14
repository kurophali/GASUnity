using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IGameplayAbility
{
    #region INSTANCING
    public static Dictionary<string, int> gAbilityNameToIndex { get; private set; }
    public static int gAbilityCount { get; private set; }
    public static List<Type> gAbilityTypes { get; private set; }
    public static List<IGameplayAbility> gAbilityDefaults { get; private set; }


    static IGameplayAbility()
    {
        gAbilityNameToIndex = new Dictionary<string, int>();
        gAbilityCount = 0;
        gAbilityTypes = new List<Type>();

        // Get all the non-abstract ability sub-classes
        foreach (var domain_assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            List<Type> assemblyTypes = new List<Type>();
            // Iterate through every type in this assembly
            foreach (Type type in domain_assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(IGameplayAbility)) && !type.IsAbstract)
                {
                    assemblyTypes.Add(type);
                    if (gAbilityNameToIndex.ContainsKey(type.Name))
                    {
                        throw new InvalidOperationException("Can't have ability classes of same name");
                    }

                    gAbilityNameToIndex[type.Name] = gAbilityCount;

                    gAbilityCount += 1;
                }
            }

            gAbilityTypes.AddRange(assemblyTypes);
        }

        // Pre-allocate the gAbilityDefaults for the functions used
        gAbilityDefaults = new List<IGameplayAbility>();
        for (int i = 0; i < gAbilityTypes.Count; ++i) {
            IGameplayAbility defaultGA = (IGameplayAbility)Activator.CreateInstance(gAbilityTypes[i]);
            gAbilityDefaults.Add(defaultGA);
        }
    }
    public string mAbilityTypeName { get; private set; }
    protected IGameplayEntity mAbilityOwner { get; private set; }
    public IGameplayAbility()
    {
        mAbilityTypeName = this.GetType().Name;
    }


    public virtual int VFValidateEntityForAbility(IGameplayEntity gameplayEntity) { return 0; }
    public virtual int VFRelease() { return 0; }
    #endregion

    #region TRIGGERING

    public int abilityState = 0;
    bool mAbilityTriggered = false;
    public Vector3 mTriggerVectorServer { get; protected set; } = new Vector3(0, 0, 0);
    public Vector3 mTriggerVectorEnemies { get; protected set; } = new Vector3(0, 0, 0);
    public Vector3 mTriggerVectorAllies{ get; protected set; } = new Vector3(0, 0, 0);
    public int SetOwner(IGameplayEntity gameplayEntity)
    {
        mAbilityOwner = gameplayEntity;
        return 0;
    }

    // W.I.P. Changing positions
    public virtual int VFOnServerUpdateItself(IGameplayEntity caster, Vector3 serverTriggerVector)
    {
        return 0;
    }
    public virtual int VFOnServerUpdateAllyRpcs(IGameplayEntity caster, Vector3 allyTriggerVector) {

        return 0; 
    }
    public virtual int VFOnServerUpdateEnemyRpcs(IGameplayEntity caster, Vector3 enemyTriggerVector)
    {

        return 0;
    }

    protected virtual int VFOnServerTriggerValidator(IGameplayEntity caster) {
        return 0; 
    }
    protected virtual int VFOnServerTriggerDetected() {
        return 0; 
    }

    protected int VFOnServerProcessTriggerVectorForFactions(Vector3 triggerVector)
    {
        mTriggerVectorAllies = triggerVector;
        mTriggerVectorEnemies = triggerVector;

        return 0;
    }

    // This is only on the server
    public int OnServerTrigger(IGameplayEntity caster, Vector3 triggerVector)
    {
        mTriggerVectorServer = triggerVector;

        int output = VFOnServerTriggerValidator(caster);
        if(output == 0)
        {
            mAbilityTriggered = true;
            VFOnServerProcessTriggerVectorForFactions(triggerVector);
        }
        return output;
    }

    public int SetAbilityState(int newStateNum)
    {
        abilityState = newStateNum;
        return 0;
    }


    #endregion
}
