using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace design
{
    public class TODO : MonoBehaviour
    {
        class main
        {
            // usunać debet?
                // przegrywamy jak nie ma kasy na rolla ani na klocki
                // roll powinien byc tanszy od najtanszego klocka
                // gra powinna się konczyć jak nie ma kasy na rolla + najtanszy klocek 
                // usunąć trzymanie klocków
            // zastanowić się czy zadania to jest dobry pomysł czy da się je prosto zrobić
            // przesuwanie klocków: 
            // duże poziomy nie da się wycelować ze względu na offset move
            // dorobić cień
            // tapanie jest niestabilne (do obracania)
            // obrócone klocki zostają w tym stanie
            // usunąc next level
            // mniej małych klocków więcej dużych

            // ==========================================

            // czy potrzebny jest wzrost cen?
            // chyba tak
            // unifikacja cen?


            // dodać duch klocka -> to powinno wyelelinować problemy z odsnapowywaniem się klocka przy cofaniu palca
            // każdy klocek ma dubel pod sobą (kopie z połową opa, schowaną)
            // gdy snap jest spełniony to włączamy dubla i przesuwamy jego) 
            // jak snap się koczy wyłączamy dubla 
            // jak puszcamy to przesuwamy w zesnapowane miesjce, wyłączamy dubla 

            // dobrze byłoby zaadresować problem z wiecznym progresem 
            // misje: jakiś seed z celem -> level
            // system dodawania poziomów
            // bierzemy seeda i dopasowujemy mu ilość kasy pocz. plus liczbę poziomów do wykonania + koszt startowy rerolla
            // ilość klocków w rerollu, etc

            // reklamy 
            //// bannery: app-ads.txt
            //// wersja bez 
            //// menu item -> remove 
            ///

            // =========================
            // spinoff
            // dostajemy wyrollowane klocki plus mapę, wybieramy na ile je zrobimy
            // jak się uda dostajemy hajs na next jak się nie uda to przegrywamy wogóle 
        }

        class ideas
        {

        }

        class Pytanai
        {
            // czy klocki się zmieniają z czasem -> nie 
        }

        class Reklamy
        {
            // intersitial kiedy? co 5 leveli
            // jak gracz przejdzie poziom to pauza, intersitial na ryj i jak się skońćzy to odpauzowujemy 
            
            // intersitial + wersja darmowa ->  balans -> wrzucić do sklepu -> app ads 
        }

        class Content
        {
            class LevelDesign
            {
                void Pytania()
                {
                    // ile ma byc poziomów? -> ~ 50
                    // czy ma być stała liczba poziomów? -> TAK
                    // czy zawsze zaczynamy tym samym poziomem -> TAK
                }
            }
            class Balans
            {
                // koszt klocków
                // częstość klocków
                // graf poziomów
                // poziomy
                // nagrody za poziomy
                // progresja kosztu klocków
                // 

                void Pytania()
                {
                    // czy chcemy progresji kosztu klocków -> tak
                }
            }
            class Tutorial
            {
                void Pytania()
                {
                    // czy wprowadzamy tutorial? -> na razie nei
                }

                void PropozycjeTutorialu()
                {
                    // nie ma tutorialu po prostu początek jest prosty zawsze 
                    // tutorial ogarnę jak zobaczę że jest konieczny 

                    // jak gracz nic nie ułożył od jakiegoś czasu 
                    // jeśli nie ma pasującego klocka to mrugamy rerollem
                    // jeśli jest pasujący klocek to animujemy jego cień
                    // 
                }
            }

            class Workspace
            {
                // jak musi być zaadaptowany początek żeby był dobry dla początkujących 
                // musi byc trochę hajsu, musza być w miarę proste poziomy, pierwszy powinein być bardzo prosty -> dwa klocki
                // co jest potrzebne do skonczenia gry
                // levele, balans, 

                // co składa się na progresję? 

                // bazowy poziom trudności -> kilka zmieniających sie prostych poziomów
                // wtedy dokładamy progresję
                // proste klocki na początek -> nowy level nowe klocki
                // wszystkie klocki trzeba podzielić na kategorie zaawansowania (trudności) 
                // kiedy wchodzą kategorie ? wraz z zwiększającem się poziomem trudności

                // jak pokazać że klocki zużywają pieniądze

                // animacja kasy opcje: 
                    // duszek zmiany
                    // zmieniające się monetki
                    //
            }
        }
    }
}