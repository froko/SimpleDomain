namespace GiftcardSample.Events
open SimpleDomain
open System

type GiftcardCreated = { CardId : Guid; CardNumber : int; InitialBalance : decimal; ValidUntil : DateTime } 
    with interface IEvent

type GiftcardActivated = { CardId: Guid }
    with interface IEvent

type GiftcardRedeemed = { CardId: Guid; Amount : decimal }
    with interface IEvent

type GiftcardLoaded = { CardId: Guid; Amount : decimal }
    with interface IEvent