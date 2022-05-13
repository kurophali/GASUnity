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
            foreach(Type type in domain_assembly.GetTypes())
            {
                if(type.IsSubclassOf(typeof(IGameplayAbility)) && !type.IsAbstract)
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


    public virtual int VFValidateEntityForAbility(IGameplayEntity gameplayEntity) {return 0; }
    public virtual int VFRelease(){ return 0;}
    #endregion

    #region TRIGGERING

    public int abilityState = 0;
    bool mAbilityTriggered = false;
    public Vector3 mTriggerVector { get; protected set; } = new Vector3(0, 0, 0);
    public int SetOwner(IGameplayEntity gameplayEntity)
    {
        mAbilityOwner = gameplayEntity;
        return 0;
    }
    public virtual int VFOnClientTrigger(Vector3 triggerVector)
    {
        return 0;
    }
    public virtual Vector3 VFProcessTriggerVectorForAllies(Vector3 triggerVector)
    {
        return triggerVector;
    }

    public virtual Vector3 VFProcessTriggerVectorForEnemies(Vector3 triggerVector)
    {
        return triggerVector;
    }

    // W.I.P. Changing positions
    protected virtual int VFOnUpdate() {
        return 0; 
    }

    protected virtual int VFTriggerValidator(IGameplayEntity caster) {
        return 0; 
    }
    protected virtual int VFOnTriggerDetected() {
        return 0; 
    }

    // This is only on the server
    public int Trigger(IGameplayEntity caster, Vector3 triggerVector)
    {
        mTriggerVector = triggerVector;

        int output = VFTriggerValidator(caster);
        if(output == 0)
        {
            mAbilityTriggered = true;
        }
        return output;
    }

    public int SetAbilityState(int newStateNum)
    {
        abilityState = newStateNum;
        return 0;
    }

    public int Update()
    {
        int result = 0;
        if (mAbilityTriggered)
        {
            result = VFOnTriggerDetected();
            mAbilityTriggered = false;
        }

        result = VFOnUpdate();

        return result;
    }
    #endregion
}
