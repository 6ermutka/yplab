def b(a):
    k = []
    current_number = ''
    for char in a:
        if char.isdigit():
            current_number += char
        else:
            k.append(int(current_number))
            k.append(char)
            current_number = ''
    k.append(int(current_number))
    i = 1
    while i < len(k) - 1:
        if k[i] == '*':
            k[i - 1] = k[i - 1] * k[i + 1]
            del k[i:i + 2]
        elif k[i] == '/':
            if k[i + 1] == 0:
                return "infiniti"
            k[i - 1] = k[i - 1] // k[i + 1]
            del k[i:i + 2]
        else:
            i += 1
    result = k[0]
    i = 1
    while i < len(k) - 1:
        if k[i] == '+':
            result += k[i + 1]
        elif k[i] == '-':
            result -= k[i + 1]
        i += 2
    return result
a = input(": ")
c = b(a)
print("answer:", c)
