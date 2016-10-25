using System;
using System.Collections.Generic;

namespace MyTCGLib
{
   public class BaseHand
   {
      public List<BaseCard> Cards { get; protected set; }

      public BaseHand()
      {
         Cards = new List<BaseCard>();
      }

      /// <summary>
      /// Add a card to the hand. Calls the ApplyAddToHandEffect event on the card.
      /// </summary>
      /// <param name="card"></param>
      public void AddCardToHand(BaseCard card)
      {
         if (card == null)
         {
            throw new ArgumentNullException(nameof(card));
         }

         Cards.Add(card);

         card.ApplyAddToHandEffect();
      }

      /// <summary>
      /// Removes the card from hand.
      /// </summary>
      /// <returns>The card from hand or null if index is invalid</returns>
      /// <param name="index">Index.</param>
      public BaseCard PopCard(int index)
      {
         BaseCard card = PeekCard(index);
         if (card == null)
         {
            return null;
         }

         Cards.Remove(card);
         return card;
      }

      /// <summary>
      /// Removes the card from hand.
      /// </summary>
      /// <returns>The card from hand or null if index is invalid</returns>
      /// <param name="index">Index.</param>
      public bool RemoveCard(BaseCard card)
      {
         if (Cards.Contains(card))
         {
            Cards.Remove(card);
            return true;
         }

         return false;
      }

      /// <summary>
      /// Returns the card at the specified index
      /// </summary>
      /// <returns>The card.</returns>
      /// <param name="index">Index.</param>
      public BaseCard PeekCard(int index)
      {
         if (index < 0 || index >= Cards.Count)
         {
            return null;
         }

         return Cards[index];
      }

      public override string ToString()
      {
         return string.Format("[Hand={0} cards]", Cards.Count);
      }
   }
}
