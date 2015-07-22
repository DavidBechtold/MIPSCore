.text
main:
	li $a0, 168
	li $a1, 1024
loop: 
	beq $a0, $a1, exit
    slt $t0, $a0, $a1
    bne $t0, $zero, label
    sub $a0, $a0, $a1
    j loop
label: 
	sub $a1, $a1, $a0
    j loop 
exit: 
	add $v1, $zero, $a0               
	li $v0, 10
	syscall