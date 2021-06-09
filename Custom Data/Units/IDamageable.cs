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
    event Action<IDamageable, IDamageable, List<Element>, int> OnReceiveDamage;

    // heal events
    event Action<IDamageable, IDamageable, ModInt> OnPredictHealOther;
    event Action<IDamageable, IDamageable, ModInt> OnHealOtherCalculation;
    event Action<IDamageable, IDamageable, ModInt> OnPredictReceiveHeal;
    event Action<IDamageable, IDamageable, ModInt> OnReceiveHealCalculation;
    event Action<IDamageable, IDamageable, List<Element>, int> OnReceiveHeal;

    // damage methods
    int PredictDealDamage(int damage, IDamageable target, List<Element> elements);
    int PredictReceiveDamage(int damage, IDamageable source, List<Element> elements);
    int DealDamage(int damage, IDamageable target, List<Element> elements);
    int ReceiveDamage(int damage, IDamageable source, List<Element> elements);

    // heal methods
    int PredictHealOther(int heal, IDamageable target, List<Element> elements);
    int PredictReceiveHeal(int heal, IDamageable source, List<Element> elements);
    int HealOther(int heal, IDamageable target, List<Element> elements);
    int ReceiveHeal(int heal, IDamageable source, List<Element> elements);
}