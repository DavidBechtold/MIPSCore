.text
main:
	li $a0, 4
	jal fib     #call fib (n)
	move $t3,$v0    #result is in $t3

# End program
li $v0,10
syscall

fib:
# Compute and return fibonacci number
beqz $a0,zero   #if n=0 return 0
beq $a0,1,one   #if n=1 return 1

#Calling fib(n-1)
sub $sp,$sp,4   #storing return address on stack
sw $ra,0($sp)

sub $a0,$a0,1   #n-1
jal fib     #fib(n-1)
add $a0,$a0,1

lw $ra,0($sp)   #restoring return address from stack
add $sp,$sp,4


sub $sp,$sp,4   #Push return value to stack
sw $v0,0($sp)
#Calling fib(n-2)
sub $sp,$sp,4   #storing return address on stack
sw $ra,0($sp)

sub $a0,$a0,2   #n-2
jal fib     #fib(n-2)
add $a0,$a0,2

lw $ra,0($sp)   #restoring return address from stack
add $sp,$sp,4
#---------------
lw $s7,0($sp)   #Pop return value from stack
add $sp,$sp,4

add $v0,$v0,$s7 # f(n - 2)+fib(n-1)
jr $ra # decrement/next in stack

zero:
li $v0,0
jr $ra
one:
li $v0,1
jr $ra

