

var cards = "234567891JQKA";    //10 replaced by 1 because it's easier to find the first character and there is no "1" card

function PushAll(to, from)
{
    var length = from.length;
    for(var i = 0; i < length; i++)
    {
        to.push(from[i]);
    }
}

var n = parseInt(readline());   //The number of cards for player 1
var p1 = [];
for (var i = 0; i < n; i++)
{
    p1.push(readline());
}

var m = parseInt(readline());   //The number of cards for player 2
var p2 = [];
for (var i = 0; i < m; i++)
{
    p2.push(readline());
}

function Battle(p1, p2, s1, s2, turns, result)
{
    var c1 = p1.shift(), c2 = p2.shift();
    var i1 = cards.indexOf(c1[0]), i2 = cards.indexOf(c2[0]);
    s1.push(c1);
    s2.push(c2);

    if (i1 > i2)
    {
        PushAll(p1, s1.concat(s2));
        if (p2.length == 0)
        {
            result.mess = "1 " + turns;
            return false;
        }
        return true;
    }

    if (i1 < i2)
    {
        PushAll(p2, s1.concat(s2));
        if (p1.length == 0)
        {
            result.mess = "2 " + turns;
            return false;
        }
        return true;
    }

    if (p1.length <= 3 || p2.length <= 3)
    {
        result.mess = "PAT";
        return false;
    }

    s1.push(p1.shift(), p1.shift(), p1.shift());
    s2.push(p2.shift(), p2.shift(), p2.shift());
    return Battle(p1, p2, s1, s2, turns, result);
}

var result = { mess: "" };
for (var turns = 1; ; turns++)
{
    if (!Battle(p1, p2, [], [], turns, result)) { break; }
}

print(result.mess);