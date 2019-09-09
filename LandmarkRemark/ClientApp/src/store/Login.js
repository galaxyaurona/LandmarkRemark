const API_URL = 'api/Users/'
const LOGIN = 'LOGIN';
const LOGIN_INITIATED = LOGIN + '_INITIATED'
const LOGIN_SUCCESS = LOGIN + '_SUCCESS'
const LOGIN_ERROR = LOGIN + '_ERROR'
const initialState = { isAuthenticated: false, isLoading: false
                , errors: []
                , userData: {}
                , username: ''
            };

const unknowErrorAction = { type: LOGIN_ERROR, errors: ['Unknow error occured'] }

export const actionCreators = {
    // take username, return a function to beinput of bindActionCreators
    login: username => async (dispatch, getState) => {

        // does not initiate when username is empty or equal to last username sent;
        if (!username || username.trim() == "" || username.trim() == getState().login.username) return;
        try {
   
            // intiate login request
            dispatch({ type: LOGIN_INITIATED, username })
            const url = `${API_URL}/login_or_signup`
            const data = {
                username
            }

            // sending post request with stringify data
            const fetchOptions = {
                method:'POST',
                body: JSON.stringify(data),
                headers : {
                    'Content-Type': 'application/json'
                }
            }
            const response = await fetch(url,fetchOptions);

            if (response.status == 200 || response.status == 201) {
                // can safely return response
                const userData = await response.json();
                dispatch({ type: LOGIN_SUCCESS, userData })
            } 
            else {
                dispatch(unknowErrorAction)
            }


        } catch (error) {
            dispatch(unknowErrorAction)
        }

    }
}

export const reducer = (state = initialState, action) => {
    switch (action.type) {
        case LOGIN_INITIATED:
            return {
                ...state,
                username: action.username,
                isAuthenticate: false,
                isLoading: true,
                errors: [],
                userData: {},
            }
        case LOGIN_SUCCESS:
            return {
                ...state,
                isAuthenticated: true,
                isLoading: false,
                username: '',
                userData: action.userData,
                errors: []
            }
        case LOGIN_ERROR:
            return {
                ...state,
                isAuthenticated: false,
                isLoading: false,
                username: '',
                userData: {},
                errors: action.errors
            }
        default:
            return state;
    }
}