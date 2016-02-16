.text
	  main:
			li $t0, 0
			li $t1, 5
			
			loop:
				beq $t0, $t1, end
				addi $t0, $t0, 1
				j loop

			end:
				syscall
