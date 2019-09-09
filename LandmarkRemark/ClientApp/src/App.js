import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Search from './components/Search';
import Login from './components/Login';
import RequireAuth from './components/RequiredAuth'
export default () => (
  <div>

    <Layout>
      <Route exact path='/' component={RequireAuth(Home)} />
      <Route path='/Login' component={Login} />
      <Route path='/search' component={RequireAuth(Search)} />

    </Layout>
    
  </div>

);
