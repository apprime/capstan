using System;
using System.Collections.Generic;
using System.Text;

namespace Capstan.Scenario
{
    class Main
    {
        public void Intro()
        {
            /*
             
            can we make permissions automatically attach themselves to relevant events?
            Event (A) is of type Event<T>, where T is perhaps <Spell>

            All permissions (B1, B2, ...) could have implementations such as IDecide<Spell>, IDecide<Movement>.
            Event<spell> A is raised, handled in SpellFilter(F) and then each IDecide<Spell> will receive items they care about.
            But then what? We are now in a subscribe inside of IDecide<Spell> and we have no context, other than the event information itself.

            x.Subscribe(i => i.Validate(this)) ? Problem being that the A.Validate(IDecide<Spell>) does not know how many validators to look for.
            Because of this, it does not know when/if the validation is ok.

            This is the fork problem with push based code.

            A => F => B1, B2 => C cannot possibly be done without C knowing about both B1 and B2. 
            If it does, what is the benefit of using observables over simply A => C, inside C call C.B1, C.B2 ?

            Lets say we construct a form of transaction such that:
            Transaction.add() => add looks for all IDecide<Spell> and subscribes to their Validate observable. 
            Validate is already subscribing to the SpellFilter F, which means that when this event A is raised, both B1 and B2 are notified.
            They then alter Validate with true or false and emit to the new observable inside of Transaction. Once all observables have returned,
            we have a result. Now transaction can return a result to the class C that observes the transaction. A is the event, B1, B2 are IDecides

            But there is a problem. B1 and B2 will keep giving off emits through their hot observable since they are static.
            If not static, we can't actually make them register with the transaction automatically. Rules should be static anyway.
            So now we have potentially N number of transactions, waiting for X number of rules to emit to them. 
            This means we need an ID to filter responses with.
            That Id must come from A, but C does not know about A, or does it? Well, C is going to be resolving A, so it has to.
            Now we must register  A inside of C first, then propagate the event into the Transaction, then propagate it to the eventfilter.
            Meaning that the original event is handled in C, and a new even is pushed to F. Also, since the transaction must know of the rules,
            it too must be constructed or initialized before the event is pushed further.
            A => C => Transaction => Filter => rules => TransactionFilter => Transaction => C
            This seems really complicated and rather inefficient compared to A => C, C:B1, C:B2.

            It gets worse.

            What if B2 should only run if a conditional value inside A is true AND B1 is also true? B1 and B2 are not synchronous processes
            They don't know about each other at all.
            Who knows that A + B1 enables B2? C, right? Or do we need some sort of additional transaction logic, such as rules ordering?
            Each Event handler(C) is going to need to configure their transaction for validation differently, depending on the order of things.
            So again we would be better of just resolving all of that logic synchronously inside of C.

            Another example:

            Player P casts fireball E (a spell) to attack two monsters M1 and M2.
            There is a handler C that will resolve this on the server-side to figure out damage done and other things.

            C receives the event E. It checks some rules for E. Player has enough mana, fireball is not on cooldown.
            E resolves.
            M1 and M2 will take E.damage. 

            M1 observes Spellfilter F. When E resolves, F will propagate E to M1, telling M1 to reduce it's own HP by 10.
            Problem 1: How does M1 know that E wants it to lower HP? Does it have one subscription for each result and event may have?

            Solution:
            It seems better to just let M1 receive a function that it inserts itself into.

            obs.Subscribe(modifier => modifier(this));

            So now we are pushing a function from the event into an entity and once we arrive at the entity we let the function modify it.

            Problem 2: M2 is a fire elemental and is immune to fireball. modifier(this) would then need to contain logic for handling all
            sorts of altered modifications there was, such as immunities, damage reductions, damage increases, additional effects and so on.

            Solution:
            each entity must implement (or inherit) functionality for all kinds of modifications.

            class CastSpellFireball
            {
                modifier(Entity this)
                {
                    this.TakeDamage(10, DamgeType.Fire);
                }
            }

            Spearfisher code => Entity observes hot observable.
            once called upon, it only reacts by injecting itself into the method that was pushed to Entity inside observable.
            So we actually have the observable doing modifications to the observer, by automatically getting 1-N entities call its method.



             */



            //Scenario 1: Player Casts a Fireball at a target inside of an area
            //Scenario 2: Player sends a text message to group
            //Scenario 3: Player changes equipment and needs to notify all clients with UI update.

            var area = new Area();

            var player = new Player();
            area.AddEntity(player);

            //We do not care if player is allowed to cast this spell or not.
            var evt = new CastSpell<Fireball> { Caster = player, Target = new Location { X = 13, Y = 37 } };

            //Area needs to pick up on this CastSpell.
            //Player is Entity, it does the casting. It is source?

            //Player generates event, puts event in its own pool?
            //Pool 

            //All observers of source, notified.
            //Notificaitons filtered.
            //Area observes player, is not filtered out, receives event.

            //Area should get the spell, Materialize it.
            //Materialized fireball is calculated hitting entities.
            //Each entity should be notified it takes damage.


        }


    }
}
