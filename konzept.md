# Detektai Jam

Thema: Alles ist verbunden.
Wir sind ein Detektiv und müssen eine gestohlene Marmelade finden. Dabei verbinden wir die einzelnen Hinweise
miteinander.

### Coregameloop:

- Detektiv führt Gespräche (mit Opfer, Verdächtigen, Zeugen)
- Aus Gesprächen erfahren wir Hinweise
- Die neuen Hinweise werden als Notizen/Bilder/Zettelchen an einer Pinnwand dargestellt (falls sie noch nicht auf der
  Pinnwand enthalten sind)
- Durch das richtige Verknüpfen der Hinweise werden neue Dialoge freigeschalten
- Diese schalten wiederrum neue Hinweise frei

Szenen/Ansichten/Systeme die wir benötigen:

- Dialoge bzw. Dialogsystem
- Pinnwand
    - Hinweise (Zettel) die auf der Pinnwand automatisch platziert werden
    - 1/3 des Screens ist ein Notizblock, hier werden Notizen, Buttons, Bilder angezeigt
- Hinweisansicht
    - Beim Klick auf einen Hinweis wird die Notiz der Pinnwand ausgefüllt
        - 2/3: Notizen, die sich der Detektiv zum Hinweis gemacht hat. Wenn die Notiz eine Person ist, enthällt sie
          einen Button "In die Detektei bestellen!"
        - 1/3: Ein Bild, dass den Hinweis visuell  (entweder das Icon des Hinweises, oder eine fancy Nahaufnahme)
- Verbindungen
    - Eine Verbindung wird erstellt, indem ein Hinweis angeklickt wird und der Finger/Cursor zu einem folgenden Hinweis
      gezogen wird.
    - Wenn die Verbindung korrekt ist, wird der Hinweis, der einen Dialog freigeschalten, hat durch einen Effekt
      hervorgehoben.
    - Der Button zum Starten des Dialogs (Button "In die Detektei bestellen!") wird auch durch einen Effekt
      hervorgehoben
    - Falsche Verbindungen werden von der Pixelschere automatisch durchgeschnitten.

Modifyer

- Gameboy Style: Pixelshader der alles in 4 grünstufen einfärbt
- Sounds: Die Aktionen des Spielers verändern den Hintergrundsound
- Eh... what?: Am Ende des Falls gibt es einen Plottwist (oder so)