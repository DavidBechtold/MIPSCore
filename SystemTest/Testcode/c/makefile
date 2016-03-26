#
# Makefile for the MIPSCore testprograms
#
# Targets:
#   all         generates object dumps off all testprogramms
#

FILENAMES = bubblesort.objdump	quicksort.objdump sevenSegmentDisplay.objdump ledRotator.objdump
OBJECTS   = bubblesort.o quicksort.o sevenSegmentDisplay.o ledRotator.o

all: $(FILENAMES)

%.objdump: %.out
	mips-elf-objdump -D $< > $@

%.out: %.o
	mips-elf-ld -T MIPSCore.ld -o $@ $<
	
%.o: %.c
	mips-elf-gcc -c $<
	

clean:
	rm -f $(FILENAMES) $(OBJECTS)


