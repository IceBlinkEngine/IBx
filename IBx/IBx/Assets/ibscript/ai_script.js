// ActionToTake must be set along with CombatTarget SpellToCast, as applicable
var targets = inputs.GetValue("targets");
var cr_category = inputs.GetValue("cr_category");

var rand = new System.Random();
var log = System.Console.WriteLine;

var adjacentEnemies = [];

// targets[].playerClass could be used to target casters if desired as well

// Loop over all targets once, recording information about each
var closestTargetIdx = -1;
var distancesTable = {};
var minimumDistance = 999999;
for (var idxTarget = 0; idxTarget < targets.length; idxTarget++) {
    var target = targets[idxTarget];
    if (target.distance <= 1) {
        adjacentEnemies.push(idxTarget);
    }

    // Remember the distances of each target "grouped" by their distance
    if (distancesTable[target.distance] == null) {
        distancesTable[target.distance] = [];
    }
    distancesTable[target.distance].push(idxTarget);

    // Record the distance to the closest PC(s)
    if (target.distance < minimumDistance) {
        minimumDistance = target.distance;
    }
}


var ActionToTake = "Move";
var CombatTarget = targets[0].name;

log(targets[0].name);
log(targets[1].name);
log(targets[2].name);
log(targets[3].name);

// If I find an adjacent enemy, 
if (adjacentEnemies.length > 0) {
    // Select a random one from the ones adjacent to me
    // and attack him
    var randomIndex = rand.Next(adjacentEnemies.length);
    log("adjacentEnemies: yes:" + randomIndex + " / " + adjacentEnemies.length + " / " + adjacentEnemies[randomIndex]);
    ActionToTake = "Attack";
    CombatTarget = targets[adjacentEnemies[randomIndex]].name;
} else {
    log("adjacentEnemies: no");
    // If there is not an adjacent enemy 
    if (cr_category == "Ranged") {
        // If I have a ranged weapon, switch to ranged and attack a random enemy
        var randomIndex = rand.Next(targets.length);
        log("ranged: yes" + randomIndex);
        ActionToTake = "Attack";
        CombatTarget = targets[randomIndex].name;
    } else {
        // If I do not have a ranged weapon,
        // find all enemies at the closest distance and choose a random one to move toward
        log("minimumDistance:" + minimumDistance);
        log("distancesTable:" + distancesTable);
        var closestEnemies = distancesTable[minimumDistance];
        var randomIndex = rand.Next(closestEnemies.length);
        log("ranged: no" + randomIndex + " / " + closestEnemies.length);
        ActionToTake = "Move";
        CombatTarget = closestEnemies[randomIndex].name;
    }
}

outputs.SetValue("ActionToTake", ActionToTake);
outputs.SetValue("CombatTarget", CombatTarget);