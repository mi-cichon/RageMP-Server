let itemsJson = `{
    "items":{
        "0":{
            "name": "Woda",
            "width": "1",
            "height": "2",
            "url": "img/water.png"
        },
        "1":{
            "name": "Chleb",
            "width": "2",
            "height": "1",
            "url": "img/bread.png"
        },
        "2":{
            "name": "Jabłko",
            "width": "1",
            "height": "1",
            "url": "img/apple.png"
        },
        "3":{
            "name": "Kamień",
            "width": "3",
            "height": "3",
            "url": "img/stone.png"
        },
        "4":{
            "name": "Drewno",
            "width": "2",
            "height": "2",
            "url": "img/wood.png"
        },
        "5":{
            "name": "Skóra zająca",
            "width": "2",
            "height": "2",
            "url": "img/hunter/rabbit.png"
        },
        "6":{
            "name": "Skóra jelenia",
            "width": "4",
            "height": "2",
            "url": "img/hunter/deer.png"
        },
        "7":{
            "name": "Skóra dzika",
            "width": "3",
            "height": "3",
            "url": "img/hunter/boar.png"
        },
        "8":{
            "name": "Skóra kojota",
            "width": "4",
            "height": "3",
            "url": "img/hunter/coyote.png"
        },
        "9":{
            "name": "Skóra pumy",
            "width": "4",
            "height": "3",
            "url": "img/hunter/puma.png"
        },
        "10":{
            "name": "Skóra sasquatcha",
            "width": "4",
            "height": "4",
            "url": "img/hunter/sasquatch.png"
        },
        "11":{
            "name": "Kanister z benzyną",
            "width": "3",
            "height": "3",
            "url": "img/canister.png"
        },
        "12":{
            "name": "Rozpuszczalnik Nitro",
            "width": "1",
            "height": "2",
            "url": "img/nitro.png"
        },
        "900":{
            "name": "Fioletowy bratek",
            "width": "1",
            "height": "1",
            "url": "img/gardener/bfiolet.png"
        },
        "901":{
            "name": "Różowy bratek",
            "width": "1",
            "height": "1",
            "url": "img/gardener/broz.png"
        },
        "902":{
            "name": "Żółty bratek",
            "width": "1",
            "height": "1",
            "url": "img/gardener/bzolte.png"
        },
        "903":{
            "name": "Figowiec",
            "width": "1",
            "height": "2",
            "url": "img/gardener/figowiec.png"
        },
        "904":{
            "name": "Dracena",
            "width": "1",
            "height": "2",
            "url": "img/gardener/dracena.png"
        },
        "1000":{
            "name": "Wędka",
            "width": "5",
            "height": "1",
            "url": "img/fisherman/rod.png"
        },
        "1001":{
            "name": "Amur",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slodkowodne/amur.png"
        },
        "1002":{
            "name": "Jesiotr",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slodkowodne/jesiotr.png"
        },
        "1003":{
            "name": "Karp",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slodkowodne/karp.png"
        },
        "1004":{
            "name": "Lin",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slodkowodne/lin.png"
        },
        "1005":{
            "name": "Lipień",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slodkowodne/lipien.png"
        },
        "1006":{
            "name": "Karaś",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/slodkowodne/karas.png"
        },
        "1007":{
            "name": "Okoń",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/slodkowodne/okon.png"
        },
        "1008":{
            "name": "Sum",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/slodkowodne/sum.png"
        },
        "1009":{
            "name": "Szczupak",
            "width": "3",
            "height": "1",
            "url": "img/fisherman/slodkowodne/szczupak.png"
        },
        "1010":{
            "name": "Tobiasz",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/slonowodne/tobiasz.png"
        },
        "1011":{
            "name": "Szprot",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/slonowodne/szprot.png"
        },
        "1012":{
            "name": "Krąp",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/slonowodne/krąp.png"
        },
        "1013":{
            "name": "Dorsz",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slonowodne/dorsz.png"
        },
        "1014":{
            "name": "Belona",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slonowodne/belona.png"
        },
        "1015":{
            "name": "Makrela",
            "width": "3",
            "height": "1",
            "url": "img/fisherman/slonowodne/makrela.png"
        },
        "1016":{
            "name": "Łosoś",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slonowodne/losos.png"
        },
        "1017":{
            "name": "Sieja",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slonowodne/sieja.png"
        },
        "1018":{
            "name": "Halibut",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/slonowodne/halibut.png"
        },
        "1019":{
            "name": "Ciernik",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slonowodne/ciernik.png"
        },
        "1020":{
            "name": "Flądra",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/slonowodne/fladra.png"
        },
        "1021":{
            "name": "Węgorzyca",
            "width": "2",
            "height": "1",
            "url": "img/fisherman/slonowodne/wegorzyca.png"
        },
        "1022":{
            "name": "Węgorz",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/slonowodne/wegorz.png"
        },
        "1050":{
            "name": "Stary gumowiec",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/shoe.png"
        },
        "1051":{
            "name": "Stary garnek",
            "width": "2",
            "height": "2",
            "url": "img/fisherman/pot.png"
        },
        "1052":{
            "name": "Zepsuty telefon",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/phone.png"
        },
        "1053":{
            "name": "Złoty zegarek",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/watch.png"
        },
        "1054":{
            "name": "Złoty pierścionek",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/ring.png"
        },
        "1055":{
            "name": "Zepsuty aparat",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/kamera.png"
        },
        "1056":{
            "name": "Stary nóż",
            "width": "1",
            "height": "2",
            "url": "img/fisherman/knife.png"
        },
        "1057":{
            "name": "Stare okulary",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/glasses.png"
        },
        "1058":{
            "name": "Stara skarpeta",
            "width": "1",
            "height": "1",
            "url": "img/fisherman/sock.png"
        }
    }
}`;

var itemTypes = JSON.parse(itemsJson).items;