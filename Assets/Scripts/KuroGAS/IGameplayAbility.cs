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

    public virtual int VFOnGEAddAbilityInit(IGameplayEntity gameplayEntity) { return 0; }
    public virtual int VFOnGERemoveAbilityRelease() { return 0; }
    #endregion

    #region TRIGGERING

    public int abilityState = 0;
    public Vector3 mTriggerVector { get; protected set; } = new Vector3(0, 0, 0);

    public int SetOwner(IGameplayEntity gameplayEntity)
    {
        mAbilityOwner = gameplayEntity;
        return 0;
    }

    // W.I.P. Changing positions
    public virtual int VFOnServerUpdateAbility(in IGameplayEntity caster, Vector3 serverTriggerVector){return 0;}
    public virtual int VFOnServerUpdateAbilityForAllies(in IGameplayEntity caster, Vector3 allyTriggerVector) {return 0; }
    public virtual int VFOnServerUpdateAbilityForEnemies(in IGameplayEntity caster, Vector3 enemyTriggerVector) { return 0;}

    protected virtual int VFOnServerTriggerValidator(IGameplayEntity caster) {
        return 0; 
    }
    protected virtual int VFOnServerTriggerDetected() {
        return 0; 
    }

    // This is only on the server
    public int OnServerTrigger(IGameplayEntity caster, Vector3 triggerVector)
    {
        this.mTriggerVector = triggerVector;

        if (mOverridden)
        {
            Debug.Log("Ability is overridden by another");
        };

        int output = VFOnServerTriggerValidator(caster);
        if(output == 0)
        {
            output = VFOnServerTriggerDetected();
        }

        return output;
    }

    public int SetAbilityState(int newStateNum)
    {
        abilityState = newStateNum;
        return 0;
    }


    #endregion

    #region OVERRIDING
    public IGameplayAbility mOverrider { get; private set; }
    protected bool mOverridden { get; private set; } = false; // If this is true the ability will not execute OnTriggerDetected or OnUpdateXXX
    protected int Override(IGameplayAbility ability)
    {
        if (mOverridden)
        {
            Debug.Log("Can't override an already overridden ability");
            return 1;
        }

        mOverrider = ability;
        mOverridden = true;

        return 0;
    }

    protected int OverrideRelease(IGameplayAbility ability)
    {
        if(mOverrider == null || mOverridden == false)
        {
            Debug.Log("Can't un-override the ability when it's not overridded");
            return 1;
        }
        if (ability != mOverrider)
        {
            Debug.Log("Can't un-override the ability from abilities that are not the overrider");
            return 2;
        }

        mOverrider = null;
        mOverridden = false;

        return 0;
    }
    #endregion
}
