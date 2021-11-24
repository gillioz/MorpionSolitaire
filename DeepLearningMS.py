import torch


class MSDataFrame:
    '''
    The class MSDataFrame (where 'MS' stands for Morpion Solitaire)
    stores a certain number of mini-batches in an array,
    iterates over that array, and updates the mini-batches
    by applying transformations or replacing them with new ones
    after a full cycle
    '''
    
    
    def __init__(self, batch_fct, repeat = 3, size = 100):
        '''
        Constructor for the class MSDataFrame
        takes as argument a function to create a mini-batch,
        the number of times a mini-batch is used (times 8 orientations)
        and an approximate number of mini-batches to be stored.
        
        The exact number of mini-batches stored is given by the formula
            (8 * repeat * n) + 1
        where n is the result of the integer division
            n // (8 * repeat)
        '''
        self.batch_fct = batch_fct
        self.cycle_length = 8 * repeat
        self.length = (size // self.cycle_length) * self.cycle_length + 1
        self.iterator = 0
        self.cycle = 1
        self.data = []
        for i in range(self.length):
            self.data.append(batch_fct())
        self.n_batches = self.length
        
	
    def info(self):
        '''
        Returns some info about the current dataframe
        '''
        print('Number of mini-batches stored:', self.length)
        print('Number of times a mini-batch is used:', self.cycle_length)
        print('Number of mini-batches created:', self.n_batches)
    
    def read(self):
        '''
        This method return the current mini-batch,
        and then iterates to the next one after
        applying some transformations or replacing
        it with a new one
        '''
        return self.data[self.iterator]
    
    def iterate(self):
        '''
        This method iterates to the next mini-batch
        after applying some transformations
        to the current one or replacing it with a new one
        if it has been used often enough
        '''
        # at the end of a cycle, update the current mini-batch
        if self.cycle == 0:
            self.data[self.iterator] = self.batch_fct()
            self.n_batches += 1
        # every fourth iteration apply mirror transformation to current mini-batch
        elif self.cycle % 4 == 0:
            self.batch_flip()
        # any other time apply rotation to current mini-batch
        else:
            self.batch_rotate()
        # increment cycle and iterator
        self.cycle = (self.cycle + 1) % self.cycle_length
        self.iterator = (self.iterator + 1) % self.length
    
    def batch_rotate(self):
        '''
        Applies 90 degrees rotation
        to the current mini-batch
        '''
        self.data[self.iterator] = (torch.rot90(self.data[self.iterator][0], 1, [-2,-1]),
                                    self.data[self.iterator][1])
        pass
    
    def batch_flip(self):
        '''
        Applies mirror symmetry (horizontal flip)
        to the current mini-batch
        '''
        self.data[self.iterator] = (torch.flip(self.data[self.iterator][0], [-1]),
                                    self.data[self.iterator][1])
        pass
    