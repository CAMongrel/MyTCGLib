using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTCGLib
{
   /// <summary>
   /// Contains a list of cards, forming a deck. The next card to draw would be the first one in the list (index 0)
   /// </summary>
   public class BaseDeck
   {
      public List<BaseCard> Deck { get; private set; }
    
      public BaseDeck ()
      {
         Deck = new List<BaseCard> ();
      }

      public void SetDeck(IEnumerable<BaseCard> cards)
      {
         Deck.Clear ();
         Deck.AddRange (cards);
      }

      public void Shuffle(Random rnd = null)
      {
         if (rnd == null)
         {
            rnd = new Random();
         }

         List<BaseCard> newDeck = new List<BaseCard>(Deck.Count);
         for (int i = 0; i < newDeck.Capacity; i++)
         {
            int idx = rnd.Next(Deck.Count);
            newDeck.Add(Deck[idx]);
            Deck.RemoveAt(idx);
         }
         Deck = newDeck;
      }

      public BaseCard DrawCard()
      {
         if (Deck.Count == 0)
         {
            return null;
         }

         BaseCard card = Deck[0];
         Deck.RemoveAt(0);

         // Apply possible draw effect
         card.ApplyDrawEffect();

         return card;
      }

      public override string ToString()
      {
         return string.Format("[Deck={0} cards remaining]", Deck.Count);
      }
   }
}

