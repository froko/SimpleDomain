namespace GiftcardSample.Commands
open SimpleDomain
open System

type CreateGiftcard = { CardNumber : int; InitialBalance : decimal; ValidUntil : DateTime } with interface ICommand
type ActivateGiftcard = { CardId : Guid; } with interface ICommand
type RedeemGiftcard = { CardId : Guid; Amount : decimal } with interface ICommand
type LoadGiftcard = { CardId : Guid; Amount : decimal } with interface ICommand