import numpy as np
import torch
from PIL import Image
from tqdm.notebook import trange


def MSvisualize(grid):
    '''
    This is a simple function that displays
    an image of the grid
    '''
    return Image.fromarray(~np.array(torch.squeeze(grid)).astype('bool'))


class MSDataFrame:
    '''
    The class MSDataFrame (where 'MS' stands for Morpion Solitaire)
    stores a certain number of mini-batches in an array,
    iterates over that array, and updates the mini-batches
    by applying transformations or replacing them with new ones
    after a given number of steps
    '''
    
    def __init__(self, batch_fct = None, batch_size = 100,
                 repeat = 0, length = 100,
                 data = None, labels = None):
        '''
        Constructor for the class MSDataFrame
        takes as argument a function to create a mini-batch,
        the size of the mini-batches,
        the number of mini-batches to be stored,
        and the number of times a mini-batch is used
        (in 8 different orientations, so typically a multiple of 8).
        
        By default the initial data is constructed using batch_fct,
        but it can alternatively be loaded from a pair of files
        (one for the data, one for the labels)
        '''
        self.batch_fct = batch_fct
        self.batch_size = batch_size
        self.repeat = repeat
        self.iterator = 0
        self.cycle = 1
        self.offset = 1
        self.data = []
        if (data is not None) and (labels is not None):
            x = np.load(data)
            y = np.load(labels)
            if x.shape[0] == y.shape[0]:
                self.length = min(length, x.shape[0] // self.batch_size)
                self.batch_count = 0
                for i in range(self.length):
                    xi = torch.tensor(x[i* self.batch_size:(i+1)*self.batch_size]).float().unsqueeze(1)
                    yi = torch.tensor(y[i* self.batch_size:(i+1)*self.batch_size]).float().unsqueeze(1)
                    self.data.append((xi, yi))
                return
        self.length = length
        for i in range(self.length):
            self.data.append(batch_fct(self.batch_size))
        self.batch_count = self.length
        
	
    def info(self):
        '''
        Returns some info about the current dataframe
        '''
        print('Number of mini-batches stored:', self.length)
        print('Number of mini-batches created:', self.batch_count)
    
    def read(self):
        '''
        Returns the current mini-batch
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
            self.data[self.iterator] = self.batch_fct(self.batch_size)
            self.batch_count += 1
        # every fourth iteration apply mirror transformation to current mini-batch
        elif self.cycle % 4 == 0:
            self.batch_flip()
        # any other time apply rotation to current mini-batch
        else:
            self.batch_rotate()
        # increment iterator and cycle
        self.iterator = (self.iterator + 1) % self.length
        if self.repeat > 0:
            if self.iterator == 0:
                self.offset = (self.offset + 1) % self.repeat
                self.cycle = self.offset
            else:
                self.cycle = (self.cycle + 1) % self.repeat
    
    def batch_rotate(self):
        '''
        Applies 90 degrees rotation
        to the current mini-batch
        '''
        self.data[self.iterator] = (torch.rot90(self.data[self.iterator][0], 1, [-2,-1]),
                                    self.data[self.iterator][1])
    
    def batch_flip(self):
        '''
        Applies mirror symmetry (horizontal flip)
        to the current mini-batch
        '''
        self.data[self.iterator] = (torch.flip(self.data[self.iterator][0], [-1]),
                                    self.data[self.iterator][1])
    
    def train_model(self, net, n_epochs = 50, lr=0.01, momentum=0.9,
                    loss_func = torch.nn.MSELoss(), accuracy_func = None,
                    loss_monitoring = None, accuracy_monitoring = None):
        '''
        Train a model on the data. This takes as arguments:
         - the neural network
         - the number of iterations over all the data
         - learning rate and momentum
         - the loss function (by default mean square error)
         - a function measuring accuracy
         - arrays in which to store indermediate values of the loss and accuracy
        
        After each epoch, the function prints the current value of the loss
        (and accuracy of the last batch, if available)
        '''
#         optimizer = torch.optim.SGD(net.parameters(), lr=lr, momentum=0.9)
        # Only optimize over trainable parameters:
        # this is useful for "freezing" some parameters
        optimizer = torch.optim.SGD(filter(lambda p: p.requires_grad, net.parameters()),
                                    lr=lr, momentum=0.9)
        for epoch in trange(n_epochs):
            running_loss = 0.0
            running_accuracy = 0.0 
            for i in range(self.length):
                inputs, labels = self.read()
                optimizer.zero_grad()
                outputs = net(inputs)
                loss = loss_func(outputs, labels)
                loss.backward()
                optimizer.step()
                self.iterate()
                running_loss += loss.item()
                if accuracy_func is not None:
                    running_accuracy += accuracy_func(outputs, labels)
            running_loss /= self.length
            running_accuracy /= self.length
            if isinstance(loss_monitoring, list):
                loss_monitoring.append(running_loss)
            if accuracy_func is not None:
                print('[%d]  loss: %.3f   accuracy: %.2f' %
                      (epoch, running_loss, running_accuracy))
                if isinstance(accuracy_monitoring, list):
                    accuracy_monitoring.append(running_accuracy)
            else:
                print('[%d]  loss: %.3f' %
                      (epoch, running_loss))
   
