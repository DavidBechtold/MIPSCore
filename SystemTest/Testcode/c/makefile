#
# Makefile for the MIPSCore testprograms
#
# Targets:
#   all         generates object dumps off all testprogramms
#

FILENAMES = bubblesort.objdump	quicksort.objdump sevenSegmentDisplay.objdump ledRotator.objdump tree.objdump mult.objdump
OBJECTS   = bubblesort.o quicksort.o sevenSegmentDisplay.o ledRotator.o tree.o mult.o

all: $(FILENAMES)

%.objdump: %.out
	mips-elf-objdump -D $< > $@

%.out: %.o
	mips-elf-ld -T MIPSCore.ld -o $@ $<
	
%.o: %.c
	mips-elf-gcc -mlong64 -c $<
	

clean:
	rm -f $(FILENAMES) $(OBJECTS)


