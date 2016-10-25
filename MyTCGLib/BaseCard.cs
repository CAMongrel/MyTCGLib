using System;
using System.Collections.Generic;

namespace MyTCGLib
{
   /// <summary>
   /// Base class for all cards in the TCG.
   /// </summary>
   public class BaseCard
   {
      public string Name { get; set; }

      /// <summary>
      /// For unmodified base values
      /// </summary>
      private Dictionary<int, int> internalValues;

      /// <summary>
      /// For things like buffs, which are persistent, but may be removed at runtime
      /// </summary>
      private Dictionary<int, int> valueModifiers;

      public BasePlayer Owner { get; set; }

      public BaseCard ()
      {
         this.Name = "<Missing Name>";

         this.internalValues = new Dictionary<int, int>();
         this.valueModifiers = new Dictionary<int, int>();

         this.Owner = null;
      }

      public int GetValue(int type)
      {
         if (this.internalValues.ContainsKey(type))
         {
            return this.internalValues[type];
         }

         return 0;
      }

      public void SetValue(int type, int value)
      {
         if (this.internalValues.ContainsKey(type))
         {
            this.internalValues[type] = value;
         }
         else
         {
            this.internalValues.Add(type, value);
         }
      }

      /// <summary>
      /// Checks if the given value type is modified through runtime means
      /// </summary>
      /// <returns><c>true</c>, if value modified was modifed, <c>false</c> otherwise.</returns>
      public bool IsValueModified(int type)
      {
         return valueModifiers.ContainsKey(type);
      }

      /// <summary>
      /// Returns the modified (base + modifier) value for a specific value type
      /// </summary>
      /// <returns>The modified value.</returns>
      public int GetModifiedValue(int type)
      {
         int baseValue = GetValue((int)type);

         int modifier = 0;
         if (this.valueModifiers.ContainsKey(type))
         {
            modifier = this.valueModifiers[type];
         }

         return baseValue + modifier;
      }

      /// <summary>
      /// Sets the value modifier.
      /// </summary>
      /// <param name="type">Type.</param>
      /// <param name="valueMod">Value mod. Set to 0 to remove an existing modifier</param>
      public void SetValueModifier(int type, int valueMod)
      {
         if (valueModifiers.ContainsKey(type))
         {
            if (valueMod != 0)
            {
               valueModifiers[type] = valueMod;
            }
            else
            {
               valueModifiers.Remove(type);
            }
         }
         else
         {
            if (valueMod != 0)
            {
               valueModifiers.Add(type, valueMod);
            }
         }
      }

      /// <summary>
      /// Used to apply an effect at the beginning of a round as long as this card is active
      /// </summary>
      /// <param name="activePlayer">The player that is now active in the started round.</param>
      public virtual void ApplyStartOfRoundEffect(BasePlayer activePlayer)
      {
         Logger.Log(this + ".ApplyStartOfRoundEffect(activePlayer: " + activePlayer + ")");
      }

      /// <summary>
      /// Used to apply an effect at the end of a round as long as this card is active
      /// </summary>
      /// <param name="activePlayer">The player that is about to become inactive at the end of their round.</param>
      public virtual void ApplyEndOfRoundEffect(BasePlayer activePlayer)
      {
         Logger.Log(this + ".ApplyEndOfRoundEffect(activePlayer: " + activePlayer + ")");
      }

      // Used to apply an effect when drawing this card from the deck
      public virtual void ApplyDrawEffect()
      {
         Logger.Log(this + ".ApplyDrawEffect()");
      }

      // Used to apply an effect when this card is played from the hand to the board
      public virtual void ApplyPlayEffect()
      {
         Logger.Log(this + ".ApplyPlayEffect()");
      }

      // Used to apply an effect when this card is removed from the board
      public virtual void ApplyDeathEffect()
      {
         Logger.Log(this + ".ApplyDeathEffect()");
      }

      /// <summary>
      /// Used to apply an effect when this card has performed an action BEFORE the action happens (e.g. a minion attack move)
      /// 
      /// Return false to indicate an abort of the game flow.
      /// </summary>
      public virtual bool ApplyPreActionEffect()
      {
         Logger.Log(this + ".ApplyPreActionEffect()");
         return true;
      }

      /// <summary>
      /// Used to apply an effect when this card has performed an action AFTER the action has happened (e.g. a minion has attacked another card)
      /// </summary>
      public virtual void ApplyPostActionEffect()
      {
         Logger.Log(this + ".ApplyPostActionEffect()");
      }

      /// <summary>
      /// Used to apply an effect when this card is attacked by another card, e.g. one minion against another
      /// 
      /// Invoked BEFORE the attack has happened and any effects have been applied (e.g. damage)
      /// 
      /// Return false to indicate an abort of the game flow.
      /// </summary>
      public virtual bool ApplyPreAttackedEffect(BaseCard instigator)
      {
         Logger.Log(this + ".ApplyPreAttackedEffect(instigator: " + instigator + ")");
         return true;
      }

      /// <summary>
      /// Used to apply an effect when this card is attacked by another card, e.g. one minion against another
      /// 
      /// Invoked AFTER the attack has happened and any effects have been applied (e.g. damage)
      /// </summary>
      public virtual void ApplyPostAttackedEffect(BaseCard instigator)
      {
         Logger.Log(this + ".ApplyPostAttackedEffect(instigator: " + instigator + ")");
      }

      /// <summary>
      /// Used to apply an effect when this card is targeted by another card (or effect). Like damage or heal spells.
      /// 
      /// Invoked BEFORE the target effect has happened and any modifications have been applied (e.g. damage)
      /// 
      /// Return false to indicate an abort of the game flow.
      /// </summary>
      public virtual bool ApplyPreTargetEffect(BaseCard instigator)
      {
         Logger.Log(this + ".ApplyPreTargetEffect(instigator: " + instigator + ")");
         return true;
      }

      /// <summary>
      /// Used to apply an effect when this card is targeted by another card (or effect). Like damage or heal spells.
      /// 
      /// Invoked AFTER the target effect has happened and any modifications have been applied (e.g. damage)
      /// </summary>
      public virtual void ApplyPostTargetEffect(BaseCard instigator)
      {
         Logger.Log(this + ".ApplyPostTargetEffect(instigator: " + instigator + ")");
      }

      // Used to apply an effect when this card is added to a players hand
      public virtual void ApplyAddToHandEffect()
      {
         Logger.Log(this + ".ApplyAddToHandEffect()");
      }

      protected void CopyValuesTo(BaseCard otherCard)
      {
         otherCard.Owner = this.Owner;
         otherCard.Name = this.Name;
         otherCard.internalValues = new Dictionary<int, int>(this.internalValues);
         otherCard.valueModifiers = new Dictionary<int, int>(this.valueModifiers);
      }

      public virtual BaseCard CreateCopy()
      {
         throw new Exception("Only to be called in derived classes.");
      }

      public override string ToString()
      {
         return string.Format("[Card: Name={0}, Owner={1}]", Name, Owner);
      }
   }
}
