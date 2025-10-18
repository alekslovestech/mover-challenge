# Fullstack Engineer Challenge

## Introduction

At Mover we build, operate, and maintain complex software for operating Last Mile Transportation for Enterprise customers. This requires complex route optimization, real-time visualizations of vehicles, and large amounts of data. Our customers have big, complex logistic operations that require high availability and high performance.

## The context

At Mover, we are looking to add Electrical Vehicle support to our route optimization and are experimenting with some new API providers. We would like you to create a simple prototype to test the new API and enable us to experiment with it.

## The challenge

The prototype should support the following functionalities/commands:

### The backend requirements

#### API Integration:

Utilize the Google Routes Directions API to calculate routes.
Fetch the distance and time between multiple delivery points.

#### Route Optimization:

Implement an algorithm to find the optimal route. You can use a heuristic approach like the Nearest Neighbor Algorithm for simplicity.
Consider how the algorithm scales with the number of routes and delivery points.

#### Data Handling:

Allow input for a list of delivery addresses.
Error handling
Be thoughtful of how you handle errors in the application.

### The frontend Requirements

#### User Interface:

Create a simple, user-friendly form for inputting delivery addresses. Ok
Display the optimized route on a map using Google Maps.
Show the total distance and estimated time for the route.

#### Visualization:

Display the sequence of delivery points on the map.
Highlight the optimal route visually.
Resources:

- Google Routes API Documentation: [Google Routes API](https://developers.google.com/maps/documentation/routes)
- React Google Maps Libraries:
  [react-google-maps](https://github.com/tomchentw/react-google-maps)
- Route Optimization Algorithms:
  [Nearest Neighbor Algorithm](https://en.wikipedia.org/wiki/Nearest_neighbor_search)

## Our expectations

The purpose of this challenge is to have a piece of code that you have created to talk about. The choices of have made as part of creating the solution are important in the way that we would like for you to be thoughtful of them and are able to argue why they were made.

With regards to time you should be able to achieve a good result in 4-6 hours, but this is completely up to you. Part of this is also to understand and choose which corners to cut.
Keep it simple. It's not about perfection and less is usually more! 😊

We would like to receive the source code via GitHub or similar, and please include a readme file that explains how to demo the application.

### Our expectations during the interview:

- You spend about 30 minutes on presenting your solution
- You should expect us to ask questions along the way
- We need your deliverables at least 1 business day in advance to give us time to review and prepare.
