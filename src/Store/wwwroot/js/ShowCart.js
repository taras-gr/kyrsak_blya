function CountDecrease(id, pricePar) {
    var value = Number(document.getElementById("goodCount" + id.toString()).value);
    var price = Number(document.getElementById("commonPrice").value);
    var count = Number(document.getElementById("goodCommonCount").value);

    price -= Number(pricePar);
    count -= 1;
    if (value == 1)
        return;

    document.getElementById("goodCount" + id.toString()).value = value - 1;
    document.getElementById("commonPrice").value = Number(price);
    document.getElementById("goodCommonCount").value = count;
}

function CountIncrease(id, pricePar) {
    var value = Number(document.getElementById("goodCount" + id.toString()).value);
    var price = Number(document.getElementById("commonPrice").value);
    var count = Number(document.getElementById("goodCommonCount").value);

    value += 1;
    price += Number(pricePar);
    count += 1;

    document.getElementById("goodCount" + id.toString()).value = value;
    document.getElementById("commonPrice").value = price;
    document.getElementById("goodCommonCount").value = count;
}
