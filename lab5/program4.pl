color(blue).
color(black).
brand(buick).
brand(ford_mustang).
brand(toyota).
car(Color, Brand) :-
    color(Color),
    brand(Brand),
    (Color = blue; Brand = buick),
    (Color \= blue; Brand = ford_mustang),
    (Color \= blue; Brand = toyota).
