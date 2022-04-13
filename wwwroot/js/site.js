// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function hideCardOrAccount(obj) {
    let accountNum = document.getElementById("AccountNum");
    let cardNum = document.getElementById("CardNum");

    if (obj.value == "Checking") {
        accountNum.style.display = "none";
        cardNum.style.display = "block";
    }
    if (obj.value == "Saving") {
        cardNum.style.display = "none";
        accountNum.style.display = "block";
    }
}


function HideDivfaveColor()
{
    let faveColor = document.getElementById("faveColor");
    let breakFast = document.getElementById("breakFast");

        faveColor.style.display = "none"
        breakFast.style.display = "block"
}

function HideDivbreakFast()
{
    let avgSpeedSwallow = document.getElementById("avgSpeedSwallow");
    let breakFast = document.getElementById("breakFast");


        breakFast.style.display = "none"
        avgSpeedSwallow.style.display = "block"

}

function HideDivavgSpeedSwallow()
{
    let avgSpeedSwallow = document.getElementById("avgSpeedSwallow");
    let bestKPop = document.getElementById("bestKPop");


        avgSpeedSwallow.style.display = "none"
        bestKPop.style.display = "block"

}
function HideDivbestKPop()
{
    let dob = document.getElementById("leastCommonBirthday");
    let bestKPop = document.getElementById("bestKPop");


        bestKPop.style.display = "none"
        dob.style.display = "block"

}

const currencyOne = document.getElementById("Currency1");
const currencyTwo = document.getElementById("Currency2");
const amountone = document.getElementById("amount1");
const amounttwo = document.getElementById("amount2");
const rate1 = document.getElementById("rate");


function calculate(){
    const currency_one = currencyOne.value;
    const currency_two = currencyTwo.value;

    fetch(`https://v6.exchangerate-api.com/v6/71aa557ba4fb44bd02d5192f/latest/${currency_one}`)
    .then((res) => res.json())
    .then((data) => {
        const rate = data.conversion_rates[currency_two];
        rate1.innerText = `1 ${currency_one} = ${rate} ${currency_two}`;
        amounttwo.value =(amountone.value * rate).toFixed(2);
    });
}

currencyOne.addEventListener('change', calculate);
amountone.addEventListener('input', calculate);
currencyTwo.addEventListener('change', calculate);
amounttwo.addEventListener('input', calculate);


