import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';
import Login from './components/Login';
import RequireAuth from './components/RequiredAuth'
export default () => (
  <div>

    <Layout>
      <Route exact path='/' component={Home} />
      <Route path='/Login' component={Login} />
      <Route path='/counter' component={Counter} />

    </Layout>
    
  </div>

);
