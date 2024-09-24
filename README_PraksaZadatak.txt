Postovani,
Odlucio sam se za WinForms c# tehnologiju u visual studio posto sam nekako najbolje upoznat bas sa c# programskim jezikom. Prvobitno sam probao sa console application ali sam shvatio da bi dodavanje tacaka i njihovih koordinata bilo veoma komplikovano preko konzole. U obzir je jos dolazio i WPF (Windows Presentation Foundation), ali nisam hteo da dodajem bespotreban XAML i komplikovan data bindings za nesto sto je vrlo jednostavno. Odnosno, WinForms je bio sasvim dovoljan za zadatak.

Koristio sam ray tracing algoritam kako bih pomocu horizontalnih zraka iz tacke koju hocu da proverim, izbrojao broj preseka sa poligonom. Ovaj algoritam je mozda previse kompleksan za jednostavan konveksan poligon ali je vrlo koristan i brz za konkavan poligon. Posto sam prvo krenuo da radim sa konkavnim poligonima, krenuo sam i sa ray tracing algoritmom. (kod konveksnih poligona, broj preseka je 0, 1 ili 2, pa ovaj algoritam nije bas u potpunosti iskoriscen). 

Nazalost, nisam potpuno iskoristio WinForms, nisam koristio GUI kako bih isprogramirao ovo resenje vec sam sve resavao u c#. Razlog tome je sto sam osecao da direktno preko koda imam vecu kontrolu.

Jos jedna stvar, u zadatku je bilo navedeno da se omoguci unos broja temena mnogougla. Ja sam to uradio na nacin da korisnik klikom unosi teme po teme i da mu bude izlistan broj trenutnih temena. Naravno, mnogougao mora da ostane konveksan.



