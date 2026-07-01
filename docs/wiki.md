# Wiki Oneheim

Ce wiki décrit les systèmes de gameplay actuellement implémentés dans Oneheim.

## Vue d'ensemble

Oneheim est construit autour d'une boucle simple : explorer, survivre, récupérer des trésors, renforcer le personnage, défendre le camp de base et vaincre les boss qui apparaissent à des paliers de progression.

La carte mesure 60 colonnes par 22 lignes. Elle contient :

- un camp de base placé près du centre ;
- un donjon au nord-est ;
- le joueur au centre de la carte ;
- Armin, présent dès le début ;
- des coffres/trésors initiaux ;
- des récompenses et ennemis spécifiques dans le donjon.

## Cycle jour/nuit

Le cycle jour/nuit dépend du nombre de pas du joueur. Un cycle complet dure 100 pas.

| Palier dans le cycle | Phase | Effet sur la vision |
| ---: | --- | ---: |
| 0 | Crépuscule | -1 |
| 15 | Nuit | -2 |
| 65 | Aube | +1 |
| 80 | Jour | +1 |

Règles importantes :

- La vision est bornée entre 1 et 20.
- Le cycle ne s'applique pas lorsque le joueur est à l'intérieur d'une structure.
- Les transitions affichent un message dédié : coucher du soleil, arrivée de la nuit, lever du soleil ou nouveau jour.

## Vagues, brouillard et progression

Les pas du joueur pilotent la progression de la partie.

| Pas | Événement |
| ---: | --- |
| 8 | Première apparition d'ennemis. |
| Chaque multiple de 100, jusqu'à 1515 | Nouvelle vague : ennemis et trésors supplémentaires. |
| À partir de 400 | Les ennemis trop faibles peuvent être retirés lors d'une nouvelle vague. |
| 515 | Premier boss. |
| 1015 | Deuxième boss. |
| 1515 | Troisième boss. |
| Après 1515 | La partie se termine quand il n'y a plus de boss actif. |

Le niveau de menace est basé sur les pas : `niveau = max(1, pas / 100)`. Les ennemis sont sélectionnés dans des tables pondérées adaptées au niveau courant et aux familles d'ennemis de la partie. Les ennemis de niveau 2 ne sont disponibles qu'à partir de la deuxième vague, soit à 200 pas.

## Familles de monstres

Au début d'une partie, le jeu sélectionne 3 familles d'ennemis. La sélection est pondérée :

| Famille | Poids |
| --- | ---: |
| Morts-vivants | 5 |
| Faune sauvage | 5 |
| Hors-la-loi | 4 |
| Cultistes | 4 |
| Démons | 3 |

Les familles sélectionnées déterminent le pool d'ennemis utilisé par les vagues.

### Morts-vivants

- `LeglessZombie`
- `Skeleton`
- `Zombie`
- `ZombieCorpses`
- `ArmoredZombie`
- `PlagueGhoul`
- `Revenant`

### Faune sauvage

- `WildLittleBear`
- `WildBear`
- `Wolf`
- `RatsNest`
- `AlphaWolf`
- `SpiderNest`
- `Werewolf`

### Hors-la-loi

- `Drunkard`
- `Pickpocket`
- `Brigand`
- `OutlawSentinel`
- `Mercenary`
- `WatchtowerArcher`
- `Assassin`

### Cultistes

- `Novice`
- `Acolyte`
- `Cultist`
- `YoungPriest`
- `Zealot`
- `Priest`
- `Champion`

### Démons

- `Imp`
- `DemonSlave`
- `Hellhound`
- `HellStele`
- `Overseer`
- `HellObelisk`
- `DoomReaper`

## Boss

Les boss apparaissent aux paliers 515, 1015 et 1515 pas. À chaque palier, le jeu utilise une file de boss mélangée aléatoirement. Les boss possibles sont :

- `Lich`
- `Troll`
- `Wyvern`
- `HighPriest`
- `ChiefBrigand`
- `YannTheSilent`
- `DartTheSoulbound`
- `AzrakelTheForsaken`

Omana permet de connaître le prochain boss à venir. Les boss sont également une menace directe pour le camp : un boss adjacent aux murs suffit à considérer le camp comme attaqué.

Tant qu'un boss est vivant sur la carte, les déplacements du joueur ne décomptent plus de pas. Le cycle jour/nuit, les vagues et les paliers de progression restent donc en pause jusqu'à la mort du boss.

## Coffres, trésors et bonus

Les trésors proposent jusqu'à 3 choix. Un choix peut être :

- soin (`LifePoint`) ;
- points de vie maximum (`MaxLifePoint`) ;
- force (`Strength`) ;
- armure (`Armor`) ;
- vitesse (`Speed`) ;
- vision (`Vision`) ;
- téléportation au camp (`CampTeleport`) ;
- objet (`Item`).

Règles de génération :

- Le soin n'est pas proposé si le joueur a au moins 50 % de ses PV maximum.
- La vision est moins susceptible d'être proposée quand la vision du joueur est déjà élevée.
- Si une statistique domine fortement, le système peut favoriser un bonus dans cette statistique.
- Les valeurs augmentent avec la progression en pas.
- Les objets sont tirés dans un pool de 9 objets, en évitant certains objets spécifiques aux familles d'ennemis présentes.
- Une téléportation vers le camp peut être proposée si le joueur est loin du camp et qu'il existe une urgence : camp attaqué, PNJ présents au camp, ou joueur à 30 % de vie maximum ou moins.

## Objets

Le joueur peut transporter jusqu'à 3 objets. Si l'inventaire est plein, l'interface demande quel objet remplacer, ou permet de garder l'inventaire actuel.

Un objet déjà possédé peut être amélioré si sa valeur d'amélioration n'est pas nulle. Lors d'une amélioration :

- sa valeur augmente ;
- sa rareté peut monter jusqu'à `Legendary` ;
- l'inventaire est trié par rareté décroissante.

### Raretés

Les raretés utilisées sont :

- `Broken`
- `Common`
- `Uncommon`
- `Rare`
- `Epic`
- `Legendary`

### Liste des objets

- `CapeOfInvisibility`
- `DaggerLifeSteal`
- `GlassesOfClairvoyance`
- `BootsOfEchoStep`
- `TalismanOfTheLastBreath`
- `ThornBreastplate`
- `FeathersOfHope`
- `RoyalGuardGauntlet`
- `RoyalGuardShield`
- `BerserkerNecklace`
- `PaladinNecklace`
- `HolyBible`
- `SacredCrucifix`
- `RingOfEndurance`
- `BladeOfHeroes`
- `ShieldOfChampion`
- `FluteOfHunter`
- `EngravedFangs`
- `EnchantedPouch`
- `SealOfWisdom`
- `ProspectorKey`
- `HawkEye`
- `FidelityCard`
- `TrollMushroom`
- `OldGiantWoodenClub`
- `LuckyMillorLeftHand`
- `GrolMokbarRing`
- `TalismanOfPeace`
- `SealOfLivingFlesh`
- `StopWatch`
- `SauerkrautEffigy`
- `ButchersThornChaplet`
- `NordheimWatcherLantern`
- `ArbalestOfTheKingsValley`
- `HolyWater`
- `LightningAmulet`

### Exemples d'effets système

- `ProspectorKey` augmente le nombre de trésors placés.
- `GrolMokbarRing` augmente la variance du nombre d'ennemis.
- `TalismanOfPeace` réduit cette variance.
- `GlassesOfClairvoyance` peut relever la vision du joueur.
- `HawkEye` peut améliorer la qualité d'un bonus de vision.
- `LuckyMillorLeftHand` peut ajouter +1 à un bonus de statistique.
- `TalismanOfTheLastBreath` annule le combat quand il sauve le joueur d'un coup fatal, puis téléporte le joueur au camp avec un message dédié.

## PNJ

### Armin

Présent dès le début près du camp. Il sert de PNJ central pour les interactions du camp.

### Ichem

Apparaît au pas 66. Il vend des faveurs/bonus au joueur.

### Eber

Apparaît au pas 166. Il permet de recruter des gardes/mercenaires pour défendre le camp.

### Omana

Apparaît à partir du pas 350. Elle révèle le prochain boss.

### Urd

Apparaît à partir du pas 450. Elle propose un système de pari pour obtenir des objets.

### Ylva

Apparaît à partir du pas 550, après le premier boss et seulement si aucun boss n'est actif. Elle permet d'améliorer l'équipement.

Les PNJ de service apparaissent à l'intérieur du camp de base. Si aucune case libre n'est disponible dans le camp, le PNJ n'est pas placé à l'extérieur. Les mercenaires recrutés via Eber sont l'exception : ils sont déployés autour du camp.

## Camp de base

Le camp de base est une structure centrale avec 1000 points de vie. Il peut être attaqué par les ennemis.

Le camp est considéré sous attaque si :

- au moins 4 ennemis sont adjacents aux murs ;
- ou un boss est adjacent à un mur.

Les ennemis déjà à l'intérieur du camp ne comptent pas pour la détection d'attaque extérieure.

Armin peut réparer le camp au tarif de 1 pièce d'or pour 5 points de vie restaurés.

## Mercenaires

Les mercenaires sont recrutés via Eber. Ils sont placés sur des cases extérieures libres autour des murs du camp.

Règles :

- maximum 12 mercenaires ;
- placement uniquement sur des cases libres ;
- les mercenaires utilisent le niveau du joueur au moment du recrutement.

## Donjon

Un donjon est placé au nord-est de la carte. À l'initialisation :

- 2 trésors sont placés à l'intérieur ;
- 2 ennemis sont placés à l'intérieur si des cases libres sont disponibles ;
- les ennemis du donjon utilisent au minimum un niveau supérieur à celui du joueur, avec un plancher à 2.

Le donjon est une zone plus dense en risque/récompense que l'extérieur.

## Difficulté

La difficulté est configurable dans `gameSettings.json` avec :

- `Normal`
- `Hard`
- `Hell`

Elle influence le nombre d'ennemis et de trésors générés par les systèmes. En difficulté `Hell`, certains bonus de PV maximum ne restaurent pas automatiquement les PV actuels.

## Configuration et contrôles

Le fichier `Roguelike.Console/gameSettings.json` configure :

- la langue (`Language`) ;
- la difficulté (`Difficulty`) ;
- les touches (`Controls`).

Contrôles par défaut en français :

| Action | Touche |
| --- | --- |
| Monter | `Z` |
| Descendre | `S` |
| Aller à gauche | `Q` |
| Aller à droite | `D` |
| Choix 1 | `W` |
| Choix 2 | `X` |
| Choix 3 | `C` |
| Quitter | `Escape` |

## Conditions de fin

La séquence de fin se déclenche après le troisième palier de boss. Après 1515 pas, si aucun boss n'est présent sur la carte, le jeu affiche le message de victoire et marque la partie comme terminée.
