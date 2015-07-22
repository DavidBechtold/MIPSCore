 .text
 
# Start of list processing:
main: 
    la   $s6,mem      # Load 'mem' address into $s6
    li   $s0,0        # Clear our 'out of order' flag
 
# For each element of list: 
loop:
    lw   $t0,($s6)    # Load current value into $t0
    lw   $t1,4($s6)   # Load next value into $t1
 
    bltz $t1,redo     # If next < 0 (end), rescan list
    bge  $t1,$t0,next # If next >= current, advance to 'next'
 
    sw   $t1,($s6)    # Store next value in current memory
    sw   $t0,4($s6)   # Store current value in next memory
    li   $s0,1        # Set our 'out of ourder' flag
 
# Advance to next list element:
next: 
    addi $s6,$s6,4    # Advance $s6 to next list element
    j    loop         # Compare this element now
 
# Rescan the list:
redo: 
    beqz $s0,done     # Finish if 'out of order' flag = 0
    la   $s6,mem      # Otherwise, load start of list in $s6
    j    main         # and rescan the list
 
# Exit
done: 
    li   $v0,10       # Load up 'exit' syscall
    syscall           # Terminate application
 
    .data
 
# Our memory list is a series of words, terminated with -1
mem: .word 4, 5, 3, 2, 1, 8, 2, 2, 4, 86, 95, 123, 4,  -1

