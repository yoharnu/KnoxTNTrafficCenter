with open("KnoxTrafficCenter/Services/TDOTAPIService.cs", "r") as f:
    lines = f.readlines()

new_lines = []
skip = False
for i, line in enumerate(lines):
    if line.startswith("<<<<<<< HEAD"):
        skip = True
        continue
    elif line.startswith("======="):
        skip = False
        continue
    elif line.startswith(">>>>>>> origin/v2"):
        continue

    if not skip:
        new_lines.append(line)

with open("KnoxTrafficCenter/Services/TDOTAPIService.cs", "w") as f:
    f.writelines(new_lines)
