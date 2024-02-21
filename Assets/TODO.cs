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
            // chcemy pewnej przewidywalności kolejnych poziomów (widoczny następny poziom w rogu) 
            // release mandatory: start screen, zakonczenie, nieskończona gra, score
            // score
            // debet! -> przegrana gdy 15 s na debecie
            // zmiana koloru przy snapie
            // recyklowanie (potrzebne do usuwania niepotrzebnych klocków)
            // warunki przegranej: na debecie przez 20 sekund
            // dowolna ilość klocka w sklepie
            // restart -> give up
            // give up -> score screen
            // score screen -> restart

        // BALANS
            // ilość dodatkowych klocków proporcjonalna do wielkości poziomu
            // w sklepie powinny też być duże klocki (bo nie ma jak wypełniać dużych poziomów)
            // regulowana ilość klocków w sklepie
            // sklep powinien być większy (bo potrzeba też dużech klocków) 
            // more levels (into bigger ones) 
            // fix block order

            // upewnić się że małe klocki sa odpowiedno drogie 
            // more money for bigger levels 
            // upewnić się że duże i długo żyjące klocki sa odpowienio tanie (chcemy promować planowanie) 
            // w sklepie musi byc dość małych klocków żeby dało się łatwo kończyć kształty
            // na początku mniej klocków losowanych
            // poprawić pozycje losowania 
            // order klocków powinien rosnać wraz ze spawnowaniem
            // poprawić buga z poziomami 
        }

        class Pytanai
        {
            // czy klocki się zmieniają z czasem -> nie 
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