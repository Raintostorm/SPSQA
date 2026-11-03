using System;
using Project.Battle;
class DeferredWaveCoinsTests
{
    static void Check(bool ok,string name){if(!ok)throw new Exception(name);}
    static void Main()
    {
        var ledger=new DeferredWaveCoins();ledger.Earn(1,5);ledger.Earn(1,5);
        Check(ledger.Claim(1)==0,"No payout in casting wave");
        Check(ledger.Claim(2)==10,"5 coins for each resolved spell");
        Check(ledger.Claim(2)==0&&ledger.Claim(3)==0,"No repeated claim or later duplicate");
        ledger.Earn(3,5);ledger.Earn(4,5);Check(ledger.Claim(4)==5,"Later casts remain deferred");Check(ledger.Claim(5)==5,"Later wave pays exactly once");
        ledger.Earn(5,5);ledger.Reset();Check(ledger.Claim(6)==0,"No rewards leak into restarted run");
        ledger.Earn(0,5);ledger.Earn(1,-5);Check(ledger.Claim(100)==0,"Invalid rewards ignored");
        Console.WriteLine("PASS: deferred Mimic coins, per-cast accumulation, next-wave boundary, idempotent claims and restart cleanup.");
    }
}
