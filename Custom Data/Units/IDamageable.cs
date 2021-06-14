using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int maxHP { get; set; }
    int hp { get; set; }

    // damage events
    event Action<IDamageable, IDamageable, ModInt> OnPredictDealDamage;
    event Action<IDamageable, IDamageable, ModInt> OnDealDamageCalculation;
    event Action<IDamageable, IDamageable, ModInt> OnPredictReceiveDamage;
    event Action<IDamageable, IDamageable, ModInt> OnReceiveDamageCalculation;
    event Action<IDamageable, IDamageable, List<ElementData>, int> OnReceiveDamage;

    // heal events
    event Action<IDamageable, IDamageable, ModInt> OnPredictHealOther;
    event Action<IDamageable, IDamageable, ModInt> OnHealOtherCalculation;
    event Action<IDamageable, IDamageable, ModInt> OnPredictReceiveHeal;
    event Action<IDamageable, IDamageable, ModInt> OnReceiveHealCalculation;
    event Action<IDamageable, IDamageable, List<ElementData>, int> OnReceiveHeal;

    // damage methods
    int PredictDealDamage(int damage, IDamageable target, List<ElementData> elements);
    int PredictReceiveDamage(int damage, IDamageable source, List<ElementData> elements);
    int DealDamage(int damage, IDamageable target, List<ElementData> elements);
    int ReceiveDamage(int damage, IDamageable source, List<ElementData> elements);

    // heal methods
    int PredictHealOther(int heal, IDamageable target, List<ElementData> elements);
    int PredictReceiveHeal(int heal, IDamageable source, List<ElementData> elements);
    int HealOther(int heal, IDamageable target, List<ElementData> elements);
    int ReceiveHeal(int heal, IDamageable source, List<ElementData> elements);
}