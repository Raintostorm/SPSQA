# Shop purchase confirmation review
Approval preview only, runtime unchanged. Current view in index.html is a code reconstruction, not an actual Play Mode screenshot.
Existing runtime popup: string description, 950x600 modal, Confirm/Cancel, balance/day/sold validations. Proposed: real head or relic image, quantity, gem cost, before/after balances, Cancel and gold price action; retain all transaction validations and no actual debit until confirmation.
Built-in image_gen used. Reference shop-review-v4 style. Prompt: single full iPhone X Shop screenshot dimmed behind navy confirm popup, Skeleton King head x20 ally shards cost150 gems, wallet4225 to4075, blueCancel/gold gem150, onepurchase today. No real-money payment, purple modal, rates, new currencies or ornate styling. Full prompt in conversation tool call.

Implemented after approval in DailyShopScreen.cs, including daily shards and gold exchange. Compile and daily shop regression pass; fresh runtime visual QA pending.
